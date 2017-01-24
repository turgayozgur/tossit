using System;
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
        /// ProducerConnection property.
        /// </summary>
        public IConnection ProducerConnection => _producerConnection.Value;
        /// <summary>
        /// ConsumerConnection property.
        /// </summary>
        public IConnection ConsumerConnection => _consumerConnection.Value;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionFactory">IConnectionFactory</param>
        public ConnectionWrapper(IConnectionFactory connectionFactory)
        {
            _producerConnection = new Lazy<IConnection>(connectionFactory.CreateConnection);
            _consumerConnection = new Lazy<IConnection>(connectionFactory.CreateConnection);
        }

        /// <summary>
        /// Dispose producer and consumer connections is active.
        /// </summary>
        public void Dispose()
        {
            if (_producerConnection.IsValueCreated) ProducerConnection.Abort();
            if (_consumerConnection.IsValueCreated) ConsumerConnection.Abort();
        }
    }
}