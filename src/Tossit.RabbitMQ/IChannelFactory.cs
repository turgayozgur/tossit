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
        /// Get new RabbitMQ channel from ConsumerConnection for every call. TODO: comment for action.
        /// </summary>
        /// <param name="action">The action for the using that newly created channel.
        /// Also, this action will be calling, if the connection is reestablished after the unexpected closure.</param>
        void Channel(Action<IModel> action);
    }
}