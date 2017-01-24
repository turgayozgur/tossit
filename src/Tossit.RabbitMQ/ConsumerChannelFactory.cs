using System.Collections.Generic;
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
        /// List of created channels.
        /// </summary>
        private readonly IList<IModel> _channels = new List<IModel>();

        /// <summary>
        /// Get new RabbitMQ channel from ConsumerConnection for every call.
        /// </summary>
        public IModel Channel
        {
            get
            {
                // New channel.
                var channel = _connectionWrapper.ConsumerConnection.CreateModel();

                // Add channel to channel list.
                _channels.Add(channel);

                return channel;
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionWrapper">IConnectionWrapper</param>
        public ConsumerChannelFactory(IConnectionWrapper connectionWrapper)
        {
            _connectionWrapper = connectionWrapper;
        }
        
        /// <summary>
        /// Dispose all created channels.
        /// </summary>
        public void Dispose()
        {
            foreach (var channel in _channels)
            {
                channel.Abort();
            }
        }
    }
}