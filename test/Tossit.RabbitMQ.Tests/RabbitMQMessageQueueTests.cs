using System;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Tossit.Core;
using Xunit;

namespace Tossit.RabbitMQ.Tests
{
    public class RabbitMQMessageQueueTests
    {
        private readonly Mock<IChannelFactory> _channelFactory;
        private readonly Mock<IConsumerInvoker> _consumerInvoker;
        private readonly Mock<EventingBasicConsumer> _eventingBasicConsumer;
        private readonly Mock<IEventingBasicConsumerImpl> _eventingBasicConsumerImpl;
        private readonly Mock<IConnectionWrapper> _connectionWrapper;
        private readonly Mock<IJsonConverter> _jsonConverter;
        private readonly Mock<IModel> _channel;

        public RabbitMQMessageQueueTests()
        {
            _connectionWrapper = new Mock<IConnectionWrapper>();
            _connectionWrapper.Setup(x => x.ProducerConnection).Returns(Mock.Of<IConnection>());
            _connectionWrapper.Setup(x => x.ConsumerConnection).Returns(Mock.Of<IConnection>());

            _channelFactory = new Mock<IChannelFactory>();
            _channelFactory.Setup(x => x.Channel).Returns(Mock.Of<IModel>());

            _channel = new Mock<IModel>();
            _channel.Setup(x => x.CreateBasicProperties()).Returns(Mock.Of<IBasicProperties>());

            _connectionWrapper.Setup(x => x.ProducerConnection.CreateModel()).Returns(_channel.Object);

            _eventingBasicConsumerImpl = new Mock<IEventingBasicConsumerImpl>();
            _eventingBasicConsumer = new Mock<EventingBasicConsumer>(Mock.Of<IModel>());
            _eventingBasicConsumerImpl.Setup(x => x.GetEventingBasicConsumer(It.IsAny<IModel>())).Returns(_eventingBasicConsumer.Object);

            _consumerInvoker = new Mock<IConsumerInvoker>();

            _jsonConverter = new Mock<IJsonConverter>();
            _jsonConverter.Setup(x => x.Serialize(It.IsAny<object>())).Returns("{id:1}");
        }

        [Fact]
        public void SendByValidArgumentsShouldReturnTrue()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();

            // Act
            var result = rabbitMQMessageQueue.Send("queue.name", "Data here");

            // Arrange
            Assert.True(result);
        }

        [Fact]
        public void SendByValidArgumentsWithConfirmsActiveShouldHitConfirmSelect()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();

            // Act
            var result = rabbitMQMessageQueue.Send("queue.name", "Data here", new Tossit.Core.Options()
            {
                ConfirmIsActive = true
            });

            // Assert
            _channel.Verify(x => x.ConfirmSelect(), Times.Once);
        }

        [Fact]
        public void SendByValidArgumentsWithWorkerConfirmsShouldHitWaitForConfirmsOrDie()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();

            // Act
            var result = rabbitMQMessageQueue.Send("queue.name", "Data here", new Tossit.Core.Options()
            {
                ConfirmIsActive = true
            });

            // Assert
            _channel.Verify(x => x.WaitForConfirmsOrDie(It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public void SendByNullOrEmptyNameShouldThrowArgumentNullException()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();

            // Act
            // Arrange
            Assert.Throws<ArgumentNullException>(() => rabbitMQMessageQueue.Send("", "Data here"));
        }

        [Fact]
        public void SendByNullOrEmptyMessageShouldThrowArgumentNullException()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();

            // Act
            // Arrange
            Assert.Throws<ArgumentNullException>(() => rabbitMQMessageQueue.Send("queue.name", ""));
        }

        [Fact]
        public void ReceiveByValidArgumentsShouldReturnTrue()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();

            // Act
            var result = rabbitMQMessageQueue.Receive("queue.name", body => false);

            // Arrange
            Assert.True(result);
        }

        [Fact]
        public void ReceiveByNullOrEmptyNameShouldThrowArgumentNullException()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();

            // Act
            // Arrange
            Assert.Throws<ArgumentNullException>(() => rabbitMQMessageQueue.Receive(null, body => true));
        }

        [Fact]
        public void ReceiveByNullOrEmptyFuncShouldThrowArgumentNullException()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();

            // Act
            // Arrange
            Assert.Throws<ArgumentNullException>(() => rabbitMQMessageQueue.Receive("queue.name", null));
        }

        private RabbitMQMessageQueue GetRabbitMQMessageQueue()
        {
            return new RabbitMQMessageQueue(
                _connectionWrapper.Object,
                _jsonConverter.Object,
                _channelFactory.Object,
                _eventingBasicConsumerImpl.Object,
                _consumerInvoker.Object
            );
        }
    }
}