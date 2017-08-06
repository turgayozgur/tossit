using System;
using RabbitMQ.Client;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// Channel recovery interface.
    /// </summary>
    public interface IChannelRecovery
    {
        /// <summary>
        /// Attempt to recover channel.
        /// </summary>
        /// <param name="channel">Currently active channel.</param>
        /// <param name="recoveryAction">this action will be calling, if the connection and channel are reestablished.</param>
        void Attempt(IModel channel, Action<IModel> recoveryAction);
        /// <summary>
        /// Bind channel for recovery scenario.
        /// </summary>
        /// <param name="channel">Currently active channel.</param>
        /// <param name="recoveryAction">this action will be calling, if the connection and channel are reestablished.</param>
        void Bind(IModel channel, Action<IModel> recoveryAction);
    }
}