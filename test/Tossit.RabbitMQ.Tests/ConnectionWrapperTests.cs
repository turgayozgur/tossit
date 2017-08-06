using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Xunit;

namespace Tossit.RabbitMQ.Tests
{
    public class ConnectionWrapperTests
    {
        private readonly Mock<IConnectionFactory> _connectionFactory;
        private readonly Mock<ILogger<ConnectionWrapper>> _logger;

        public ConnectionWrapperTests()
        {
            _connectionFactory = new Mock<IConnectionFactory>();
            _connectionFactory.Setup(x => x.CreateConnection()).Returns(Mock.Of<IConnection>());

            _logger = new Mock<ILogger<ConnectionWrapper>>();
        }

        [Fact]
        public void GetProducerConnectionShouldNotBeNull() 
        {
            // Arrange
            var connectionWrapper = new ConnectionWrapper(_connectionFactory.Object, _logger.Object);

            // Act
            var producerConnection = connectionWrapper.ProducerConnection;

            // Assert
            Assert.True(producerConnection != null);
        }

        [Fact]
        public void GetConsumerConnectionShouldNotBeNull() 
        {
            // Arrange
            var connectionWrapper = new ConnectionWrapper(_connectionFactory.Object, _logger.Object);

            // Act
            var consumerConnection = connectionWrapper.ConsumerConnection;

            // Assert
            Assert.True(consumerConnection != null);
        }

        [Fact]
        public void DisposeShouldNotHitDisposeBeforeFirstCallProducerAndConsumer()
        {
            // Arrange
            var conection = new Mock<IConnection>();
            _connectionFactory.Setup(x => x.CreateConnection()).Returns(conection.Object);

            var connectionWrapper = new ConnectionWrapper(_connectionFactory.Object, _logger.Object);

            // Act
            connectionWrapper.Dispose();

            // Assert
            conection.Verify(x => x.Close(), Times.Never);
        }

        [Fact]
        public void DisposeShouldHitDisposeAfterFirstCallProducerConnection()
        {
            // Arrange
            var connection = new Mock<IConnection>();
            connection.Setup(x => x.IsOpen).Returns(true);
            _connectionFactory.Setup(x => x.CreateConnection()).Returns(connection.Object);

            var connectionWrapper = new ConnectionWrapper(_connectionFactory.Object, _logger.Object);

            // Act
            var producerConnection = connectionWrapper.ProducerConnection;
            connectionWrapper.Dispose();

            // Assert
            connection.Verify(x => x.Close(), Times.Once);
        }

        [Fact]
        public void DisposeShouldHitDisposeAfterFirstCallConsumerConnection()
        {
            // Arrange
            var connection = new Mock<IConnection>();
            connection.Setup(x => x.IsOpen).Returns(true);
            _connectionFactory.Setup(x => x.CreateConnection()).Returns(connection.Object);

            var connectionWrapper = new ConnectionWrapper(_connectionFactory.Object, _logger.Object);

            // Act
            var consumerConnection = connectionWrapper.ConsumerConnection;
            connectionWrapper.Dispose();

            // Assert
            connection.Verify(x => x.Close(), Times.Once);
        }
    }
}
