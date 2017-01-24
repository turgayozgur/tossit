using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// Consumer invoker interface for invokes consumer's registered methods.
    /// </summary>
    public interface IConsumerInvoker
    {
        /// <summary>
        /// Invoke given consumer function.
        /// </summary>
        /// <param name="func">Function to invoke.</param>
        /// <param name="ea">RabbitMQ's BasicDeliverEventArgs.</param>
        /// <param name="channel">Channel to response ack or nack.</param>
        void Invoke(Func<string, bool> func, BasicDeliverEventArgs ea, IModel channel);
    }
}