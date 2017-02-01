using Moq;
using RabbitMQ.Client;
using Xunit;

namespace Tossit.RabbitMQ.Tests
{
    public class ConsumerChannelFactoryTests
    {
        private readonly Mock<IConnectionWrapper> _connectionWrapper;
        private readonly Mock<IConnection> _connection;

        public ConsumerChannelFactoryTests()
        {
            _connectionWrapper = new Mock<IConnectionWrapper>();
            _connection = new Mock<IConnection>();

            _connectionWrapper.Setup(x => x.ConsumerConnection).Returns(_connection.Object);
        }

        [Fact]
        public void GetChannelShouldReturnAnyIChannel()
        {
            // Arrange
            var consumerChannelFactory = new ConsumerChannelFactory(_connectionWrapper.Object);

            var channel = new Mock<IModel>();
            _connection.Setup(x => x.CreateModel()).Returns(channel.Object);

            // Act
            var resultChannel = consumerChannelFactory.Channel;

            // Assert
            Assert.True(resultChannel == channel.Object);
        }

        [Fact]
        public void DisposeShouldHitChannelDispose()
        {
            // Arrange
            var consumerChannelFactory = new ConsumerChannelFactory(_connectionWrapper.Object);
            
            var channel = new Mock<IModel>();
            _connection.Setup(x => x.CreateModel()).Returns(channel.Object);

            // Act
            var channel1 = consumerChannelFactory.Channel;
            var channel2 = consumerChannelFactory.Channel;
            consumerChannelFactory.Dispose();

            // Assert
            channel.Verify(x => x.Close(), Times.Exactly(2));
        }
    }
}