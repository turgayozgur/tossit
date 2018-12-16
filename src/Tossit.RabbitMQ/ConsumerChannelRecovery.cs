using System;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Tossit.Core;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// Consumer channel recovery implementation.
    /// </summary>
    public class ConsumerChannelRecovery : IChannelRecovery
    {
        /// <summary>
        /// ConnectionWrapper field.
        /// </summary>
        private readonly IConnectionWrapper _connectionWrapper;
        /// <summary>
        /// Logger field.
        /// </summary>
        private readonly ILogger<ConsumerChannelRecovery> _logger;
        /// <summary>
        /// ITimeLoop field.
        /// </summary>
        private readonly ITimeLoop _timeLoop;
        /// <summary>
        /// Time to waiting for next attemp to try recovery.
        /// </summary>
        private readonly int RECOVERY_RETRY_SECONDS = 5;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionWrapper">IConnectionWrapper</param>
        /// <param name="logger">ILogger{ConsumerChannelRecovery}</param>
        /// <param name="timeLoop">ITimeLoop</param>
        public ConsumerChannelRecovery(IConnectionWrapper connectionWrapper,
            ILogger<ConsumerChannelRecovery> logger,
            ITimeLoop timeLoop)
        {
            _connectionWrapper = connectionWrapper;
            _logger = logger;
            _timeLoop = timeLoop;
        }

        /// <summary>
        /// Attempt to recover channel.
        /// </summary>
        /// <param name="channel">Currently active channel.</param>
        /// <param name="recoveryAction">this action will be calling, if the connection and channel are reestablished.</param>
        public void Attempt(IModel channel, Action<IModel> recoveryAction)
        {
            Attempt(channel, recoveryAction, null);
        }

        /// <summary>
        /// Bind channel for recovery scenario.
        /// </summary>
        /// <param name="channel">Currently active channel.</param>
        /// <param name="recoveryAction">this action will be calling, if the connection and channel are reestablished.</param>
        public void Bind(IModel channel, Action<IModel> recoveryAction)
        {
            channel.ModelShutdown += (sender, args) =>
            {
                // Dont do anything, if the connection shutted down with expected reasons.
                if (args.ReplyCode == 200) return;

                _logger.LogError($"Channel lost. {args.ReplyCode} {args.Cause}");

                _timeLoop.StartNew(
                    id => { Attempt(channel, recoveryAction, () => { _timeLoop.Stop(id); }); },
                    RECOVERY_RETRY_SECONDS * 1000);
            };
        }

        /// <summary>
        /// Attempt to recover channel.
        /// </summary>
        /// <param name="channel">Currently active channel.</param>
        /// <param name="recoveryAction">this action will be calling, if the connection and channel are reestablished.</param>
        /// <param name="successAction">Action to calling when connection all done.</param>
        private void Attempt(IModel channel, Action<IModel> recoveryAction, Action successAction =  null)
        {
            // Check connection.
            if (!_connectionWrapper.ConsumerConnection.IsOpen)
            {
                _logger.LogWarning($"Awaiting for the connection for the channel {channel.ChannelNumber}...");
                return;
            }

            // Check channel.
            if (!channel.IsOpen)
            {
                _logger.LogWarning($"Connection established. Awaiting for the channel {channel.ChannelNumber}...");
                return;
            }

            // Try calling the recovery method.
            try
            {
                lock (channel)
                {
                    recoveryAction(channel);
                }
                
                if(successAction != null) 
                {
                    successAction();
                }
                
                _logger.LogInformation($"Channel {channel.ChannelNumber} established.");
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex, $"Channel {channel.ChannelNumber} recovery failed.");
            }
        }
    }
}