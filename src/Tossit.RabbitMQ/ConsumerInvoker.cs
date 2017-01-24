using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// Consumer invoker for invokes consumer's registered methods.
    /// </summary>
    public class ConsumerInvoker : IConsumerInvoker
    {
        /// <summary>
        /// Invoke given consumer function.
        /// </summary>
        /// <param name="func">Function to invoke.</param>
        /// <param name="ea">RabbitMQ's BasicDeliverEventArgs.</param>
        /// <param name="channel">Channel to response ack or nack.</param>
        public void Invoke(Func<string, bool> func, BasicDeliverEventArgs ea, IModel channel)
        {
            // Get message/body.
            var body = ea.Body != null ? Encoding.UTF8.GetString(ea.Body) : string.Empty;

            // Invoke!
            var isSuccess = func(body);

            // Ack or nack.
            if (isSuccess)
            {
                channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            else
            {
                channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        }
    }
}