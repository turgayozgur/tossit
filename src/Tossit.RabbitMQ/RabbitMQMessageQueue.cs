using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Tossit.Core;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// RabbitMQ message queue implementation.
    /// </summary>
    public class RabbitMQMessageQueue : IMessageQueue
    {
        /// <summary>
        /// Main exchange name constant.
        /// </summary>
        private const string MAIN_EXCHANGE_NAME = "tossit.exchange.main";
        /// <summary>
        /// Main retry exchange name constant.
        /// </summary>
        private const string MAIN_RETRY_EXCHANGE_NAME = "tossit.exchange.main.retry";
        /// <summary>
        /// Retry queue name constant.
        /// </summary>
        private const string RETRY_QUEUE_NAME_SUFFIX = "retry";

        /// <summary>
        /// ConnectionWrapper field.
        /// </summary>
        private readonly IConnectionWrapper _connectionWrapper;
        /// <summary>
        /// JsonConverter field.
        /// </summary>
        private readonly IJsonConverter _jsonConverter;
        /// <summary>
        /// ChannelFactory field.
        /// </summary>
        private readonly IChannelFactory _channelFactory;
        /// <summary>
        /// EventingBasicConsumerImpl field.
        /// </summary>
        private readonly IEventingBasicConsumerImpl _eventingBasicConsumerImpl;
        /// <summary>
        /// SendOptions field.
        /// </summary>
        private readonly IOptions<SendOptions> _sendOptions;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionWrapper">IConnectionWrapper</param>
        /// <param name="jsonConverter">IJsonConverter</param>
        /// <param name="channelFactory">IChannelFactory</param>
        /// <param name="eventingBasicConsumerImpl">IEventingBasicConsumerImpl</param>
        /// <param name="sendOptions">IOptions{SendOptions}</param>
        public RabbitMQMessageQueue(
            IConnectionWrapper connectionWrapper,
            IJsonConverter jsonConverter,
            IChannelFactory channelFactory,
            IEventingBasicConsumerImpl eventingBasicConsumerImpl,
            IOptions<SendOptions> sendOptions)
        {
            _connectionWrapper = connectionWrapper;
            _jsonConverter = jsonConverter;
            _channelFactory = channelFactory;
            _eventingBasicConsumerImpl = eventingBasicConsumerImpl;
            _sendOptions = sendOptions;
        }

        /// <summary>
        /// Send to queue.
        /// </summary>
        /// <param name="name">Name of queue or whatever using to describing work/job/event. Same as Receive method's name.</param>
        /// <param name="message">Message string to sends to queue.</param>
        /// <returns>Returns true if data send completed successfully, otherwise returns false.</returns>
        /// <exception cref="ArgumentNullException">Throws when name or message is null.</exception>
        public bool Send(string name, string message)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

            // Create new channel for each dispatch.
            using (var channel = _connectionWrapper.ProducerConnection.CreateModel())
            {
                // Prepare channel to send messages.
                var queueName = name;
                var exchangeName = this.PrepareChannel(channel, queueName);

                // Enable publisher confirms.
                if (_sendOptions.Value.ConfirmReceiptIsActive)
                {
                    channel.ConfirmSelect();
                }

                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                // Set message as persistent.
                properties.Persistent = true;

                // Publish message.
                channel.BasicPublish(exchange: exchangeName, routingKey: queueName, basicProperties: properties,
                    body: body);

                // Wait form ack from worker.
                if (_sendOptions.Value.ConfirmReceiptIsActive)
                {
                    channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(_sendOptions.Value.ConfirmReceiptTimeoutSeconds));
                }
            }

            return true;
        }

        /// <summary>
        /// Receive messages from message queue.
        /// This method will register the given function to consume queue.
        /// </summary>
        /// <param name="name">Name of queue or whatever using to describing work/job/event. Same as Send method's name.</param>
        /// <param name="func">Receiver function.</param>
        /// <returns>Returns true if receiver method registered successfully, otherwise returns false.</returns>
        /// <exception cref="ArgumentNullException">Throws when name or func is null.</exception>
        public bool Receive(string name, Func<string, bool> func)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (func == null) throw new ArgumentNullException(nameof(func));

            // Get new channel.
            var channel = _channelFactory.Channel;

            // Prepare channel to consume messages and retry.
            var queueName = name;
            // To consume.
            var exchangeName = PrepareChannel(channel, queueName);
            // To retry.
            PrepareChannelForRetry(channel, queueName, exchangeName);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            // Create new consumer.
            var consumer = _eventingBasicConsumerImpl.GetEventingBasicConsumer(channel);

            // Register to job received event.
            consumer.Received += (model, ea) => { this.Invoke(func, ea, channel, queueName); };

            // Start consuming.
            channel.BasicConsume(queueName, autoAck: false, consumer: consumer);

            return true;
        }

        /// <summary>
        /// Declare main exchange, queue and bind.
        /// </summary>
        /// <param name="channel">Channel where the queue will be created.</param>
        /// <param name="queueName">The queue name to declare the queue.</param>
        /// <returns>Returns name of exchange that has binding for given queue name.</returns>
        private string PrepareChannel(IModel channel, string queueName)
        {
            // Declare main exchange and queue.
            channel.ExchangeDeclare(MAIN_EXCHANGE_NAME, ExchangeType.Direct, true, false);
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);            

            // Bind queue to main exchange if did not binded before.
            try
            {
                channel.QueueBind(queueName, MAIN_EXCHANGE_NAME, routingKey: queueName);
            }
            catch (ArgumentException)
            {
                // Queue already binded before.
                // ignore.
            }

            return MAIN_EXCHANGE_NAME;
        }

        /// <summary>
        /// Declare exchange, queue and bind. To handle for failed messages to retry later.
        /// </summary>
        /// <param name="channel">Channel where the retry queue will be created.</param>
        /// <param name="queueName">The queue name to route failed messages after a time for process again.</param>
        /// <param name="mainExchangeName">Name of exhange that created for handle messages.</param>
        private void PrepareChannelForRetry(IModel channel, string queueName, string mainExchangeName)
        {
            // Declare retry exchange and queue for given queueName.
            channel.ExchangeDeclare(MAIN_RETRY_EXCHANGE_NAME, ExchangeType.Direct, true, false);

            // Arguments for dead letter exchange. It is required for retry functionality.
            var args = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", mainExchangeName },
                { "x-message-ttl", _sendOptions.Value.WaitToRetrySeconds * 1000 }
            };

            // Declare queue to store messages for waiting to retry.
            channel.QueueDeclare($"{queueName}.{RETRY_QUEUE_NAME_SUFFIX}", durable: true, exclusive: false, 
                autoDelete: false, arguments: args);
            
            // Bind retry queue to retry exchange if did not binded before.
            try
            {
                channel.QueueBind($"{queueName}.{RETRY_QUEUE_NAME_SUFFIX}", MAIN_RETRY_EXCHANGE_NAME, 
                    routingKey: queueName);
            }
            catch (ArgumentException)
            {
                // Queue already binded before.
                // ignore.
            }
        }

        /// <summary>
        /// Invoke given consumer function.
        /// This method declared as an internal for test, because rabbitmq's event handler is not virtual.
        /// </summary>
        /// <param name="func">Function to invoke.</param>
        /// <param name="ea">RabbitMQ's BasicDeliverEventArgs.</param>
        /// <param name="channel">Channel to response ack or nack.</param>
        /// <param name="queueName">Queue name to route message to retry queue, if process failed.</param>
        internal void Invoke(Func<string, bool> func, BasicDeliverEventArgs ea, IModel channel, string queueName)
        {
            // Get message/body.
            var body = ea.Body != null ? Encoding.UTF8.GetString(ea.Body) : string.Empty;

            bool isSuccess;

            // Invoke!
            try
            {
                isSuccess = func(body);
            }
            catch
            {
                isSuccess = false;
            }

            // Success or not, doesn't matter. 
            // Send ack everytime and publish message to dead letter exchange for trying again.
            channel.BasicAck(ea.DeliveryTag, multiple: false);

            if (!isSuccess)
            {
                var properties = channel.CreateBasicProperties();
                // Set message as persistent.
                properties.Persistent = true;

                // Publish to retry queue to retry again after ttl.
                channel.BasicPublish(exchange: MAIN_RETRY_EXCHANGE_NAME, routingKey: queueName, 
                    basicProperties: properties, body: ea.Body);
            }
        }
    }
}