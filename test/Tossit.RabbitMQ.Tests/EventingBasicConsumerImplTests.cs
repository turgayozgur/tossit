using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace Tossit.RabbitMQ.Tests
{
    public class EventingBasicConsumerImplTests
    {
        [Fact]
        public void GetEventingBasicConsumerWithModelShouldReturnEventingBasicConsumer()
        {
            // Arrange
            var eventingBasicConsumerImpl = new EventingBasicConsumerImpl();

            // Act
            var result = eventingBasicConsumerImpl.GetEventingBasicConsumer(It.IsAny<IModel>());

            // Assert
            Assert.True(result.GetType() == typeof(EventingBasicConsumer));
        }
    }
}