using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// Consumer channel factory.
    /// </summary>
    public class ConsumerChannelFactory : IChannelFactory
    {
        /// <summary>
        /// ConnectionWrapper field.
        /// </summary>
        private readonly IConnectionWrapper _connectionWrapper;
        /// <summary>
        /// Logger field.
        /// </summary>
        private readonly ILogger<ConsumerChannelFactory> _logger;
        /// <summary>
        /// ChannelRecovery field.
        /// </summary>
        private readonly IChannelRecovery _channelRecovery;

        /// <summary>
        /// List of created channels.
        /// </summary>
        private readonly IList<IModel> _channels = new List<IModel>();

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionWrapper">IConnectionWrapper</param>
        /// <param name="logger">ILogger{ConsumerChannelFactory}</param>
        /// <param name="channelRecovery">IChannelRecovery</param>
        public ConsumerChannelFactory(IConnectionWrapper connectionWrapper,
            ILogger<ConsumerChannelFactory> logger,
            IChannelRecovery channelRecovery)
        {
            _connectionWrapper = connectionWrapper;
            _logger = logger;
            _channelRecovery = channelRecovery;
        }

        /// <summary>
        /// Get new RabbitMQ channel from ConsumerConnection for every call. TODO: comment for action.
        /// </summary>
        /// <param name="action">The action for the using that newly created channel.
        /// Also, this action will be calling, if the connection is reestablished after the unexpected closure.</param>
        public void Channel(Action<IModel> action)
        {
            // New channel.
            var channel = _connectionWrapper.ConsumerConnection.CreateModel();

            // Recovery scenario.
            _channelRecovery.Bind(channel, action);

            // Add channel to channel list.
            _channels.Add(channel);

            // Call the action for newly created channel.
            lock (channel)
            {
                action(channel);
            }            
        }

        /// <summary>
        /// Dispose all created channels.
        /// </summary>
        public void Dispose()
        {
            // Close RabbitMQ channels.
            foreach (var channel in _channels)
            {
                if (channel.IsOpen)
                {
                    channel.Close();
                    _logger.LogInformation($"Channel {channel.ChannelNumber} closed successfully.");
                }
            }
        }
    }
}