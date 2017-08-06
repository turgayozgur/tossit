using System;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// Connection wrapper.
    /// Wraps producer and consumer connections.
    /// </summary>
    public class ConnectionWrapper : IConnectionWrapper
    {
        /// <summary>
        /// Lazy ProducerConnection field.
        /// </summary>
        private readonly Lazy<IConnection> _producerConnection;
        /// <summary>
        /// Lazy ConsumerConnection field.
        /// </summary>
        private readonly Lazy<IConnection> _consumerConnection;
        /// <summary>
        /// Logger field.
        /// </summary>
        private readonly ILogger<ConnectionWrapper> _logger;

        /// <summary>
        /// ProducerConnection property. Thread safe.
        /// </summary>
        public IConnection ProducerConnection
        {
            get
            {
                var cnn = _producerConnection.Value;
                lock (cnn)
                {
                    return cnn;
                }
            }
        }
        /// <summary>
        /// ConsumerConnection property. Thread safe.
        /// </summary>
        public IConnection ConsumerConnection
        {
            get
            {
                var cnn = _consumerConnection.Value;
                lock (cnn)
                {
                    return cnn;
                }
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionFactory">IConnectionFactory</param>
        /// <param name="logger">ILogger{ConnectionWrapper}</param>
        public ConnectionWrapper(IConnectionFactory connectionFactory,
            ILogger<ConnectionWrapper> logger)
        {
            _producerConnection = new Lazy<IConnection>(connectionFactory.CreateConnection);
            _consumerConnection = new Lazy<IConnection>(connectionFactory.CreateConnection);
            _logger = logger;
        }

        /// <summary>
        /// Dispose producer and consumer connections is active.
        /// </summary>
        public void Dispose()
        {
            if (_producerConnection.IsValueCreated  && ProducerConnection.IsOpen)
            {
                ProducerConnection.Close();
                _logger.LogInformation("Producer connection closed.");
            }

            if (_consumerConnection.IsValueCreated && ConsumerConnection.IsOpen)
            {
                ConsumerConnection.Close();
                _logger.LogInformation("Consumer connection closed.");
            }
        }
    }
}