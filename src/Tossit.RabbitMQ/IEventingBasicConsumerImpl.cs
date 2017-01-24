using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// EventingBasicConsumerImpl interface to proxy of the EventingBasicConsumer.
    /// </summary>
    public interface IEventingBasicConsumerImpl
    {
        /// <summary>
        /// Get new EventingBasicConsumer instance.
        /// </summary>
        /// <param name="channel">Channel to getting EventingBasicConsumer.</param>
        /// <returns>Returns new EventingBasicConsumer instance.</returns>
        EventingBasicConsumer GetEventingBasicConsumer(IModel channel);
    }
}