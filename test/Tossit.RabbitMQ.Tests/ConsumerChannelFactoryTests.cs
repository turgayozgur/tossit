using System;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Xunit;

namespace Tossit.RabbitMQ.Tests
{
    public class ConsumerChannelFactoryTests
    {
        private readonly Mock<IConnectionWrapper> _connectionWrapper;
        private readonly Mock<IConnection> _connection;
        private readonly Mock<ILogger<ConsumerChannelFactory>> _logger;
        private readonly Mock<IChannelRecovery> _channelRecovery;

        public ConsumerChannelFactoryTests()
        {
            _connectionWrapper = new Mock<IConnectionWrapper>();
            _connection = new Mock<IConnection>();

            _connectionWrapper.Setup(x => x.ConsumerConnection).Returns(_connection.Object);

            _logger = new Mock<ILogger<ConsumerChannelFactory>>();
            _channelRecovery = new Mock<IChannelRecovery>();
        }

        [Fact]
        public void GetChannelShouldReturnAnyIChannel()
        {
            // Arrange
            var consumerChannelFactory = new ConsumerChannelFactory(_connectionWrapper.Object, _logger.Object, _channelRecovery.Object);

            var channel = new Mock<IModel>();
            channel.Setup(x => x.IsOpen).Returns(true);
            _connection.Setup(x => x.CreateModel()).Returns(channel.Object);

            // Act
            // Assert
            consumerChannelFactory.Channel(chn => {
                Assert.True(chn == channel.Object);
            });            
        }

        [Fact]
        public void DisposeShouldHitChannelDispose()
        {
            // Arrange
            var consumerChannelFactory = new ConsumerChannelFactory(_connectionWrapper.Object, _logger.Object, _channelRecovery.Object);
            
            var channel = new Mock<IModel>();
            channel.Setup(x => x.IsOpen).Returns(true);
            _connection.Setup(x => x.CreateModel()).Returns(channel.Object);

            // Act
            consumerChannelFactory.Channel(chn => {});
            consumerChannelFactory.Channel(chn => {});
            consumerChannelFactory.Dispose();

            // Assert
            channel.Verify(x => x.Close(), Times.Exactly(2));
        }
    }
}