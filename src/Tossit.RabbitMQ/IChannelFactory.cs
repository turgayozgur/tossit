using System;
using RabbitMQ.Client;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// Channel factory interface.
    /// </summary>
    public interface IChannelFactory : IDisposable
    {
        /// <summary>
        /// Get new RabbitMQ channel for every call.
        /// </summary>
        IModel Channel { get; }
    }
}