using System;
using RabbitMQ.Client;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// Connection wrapper interface for connection wrappers.
    /// Wraps producer and consumer connections.
    /// </summary>
    public interface IConnectionWrapper : IDisposable
    {
        /// <summary>
        /// Connection for producers. e.g: jobs
        /// </summary>
        IConnection ProducerConnection { get; }
        /// <summary>
        /// Connection for consumers. e.g: workers.
        /// </summary>
        IConnection ConsumerConnection { get; }
    }
}