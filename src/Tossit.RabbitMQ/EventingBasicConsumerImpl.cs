using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// EventingBasicConsumerImpl to proxy of the EventingBasicConsumer.
    /// </summary>
    public class EventingBasicConsumerImpl : IEventingBasicConsumerImpl
    {
        /// <summary>
        /// Get new EventingBasicConsumer instance.
        /// </summary>
        /// <param name="channel">Channel to getting EventingBasicConsumer.</param>
        /// <returns>Returns new EventingBasicConsumer instance.</returns>
        public EventingBasicConsumer GetEventingBasicConsumer(IModel channel)
        {
            return new EventingBasicConsumer(channel);
        }
    }
}