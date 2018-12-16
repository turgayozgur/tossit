using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly Mock<EventingBasicConsumer> _eventingBasicConsumer;
        private readonly Mock<IEventingBasicConsumerImpl> _eventingBasicConsumerImpl;
        private readonly Mock<IConnectionWrapper> _connectionWrapper;
        private readonly Mock<IJsonConverter> _jsonConverter;
        private readonly Mock<IModel> _channel;
        private readonly Mock<IOptions<SendOptions>> _sendOptions;
        private readonly Mock<ILogger<RabbitMQMessageQueue>> _logger;

        public RabbitMQMessageQueueTests()
        {
            _connectionWrapper = new Mock<IConnectionWrapper>();
            _connectionWrapper.Setup(x => x.ProducerConnection).Returns(Mock.Of<IConnection>());
            _connectionWrapper.Setup(x => x.ConsumerConnection).Returns(Mock.Of<IConnection>());

            _channelFactory = new Mock<IChannelFactory>();

            _channel = new Mock<IModel>();
            _channel.Setup(x => x.CreateBasicProperties()).Returns(Mock.Of<IBasicProperties>());

            _connectionWrapper.Setup(x => x.ProducerConnection.CreateModel()).Returns(_channel.Object);

            _eventingBasicConsumerImpl = new Mock<IEventingBasicConsumerImpl>();
            _eventingBasicConsumer = new Mock<EventingBasicConsumer>(Mock.Of<IModel>());
            _eventingBasicConsumerImpl.Setup(x => x.GetEventingBasicConsumer(It.IsAny<IModel>())).Returns(_eventingBasicConsumer.Object);

            _jsonConverter = new Mock<IJsonConverter>();
            _jsonConverter.Setup(x => x.Serialize(It.IsAny<object>())).Returns("{id:1}");

            _sendOptions = new Mock<IOptions<SendOptions>>();
            _sendOptions.Setup(x => x.Value.ConfirmReceiptTimeoutSeconds).Returns(It.IsAny<int>());
            _sendOptions.Setup(x => x.Value.ConfirmReceiptIsActive).Returns(false);

            _logger = new Mock<ILogger<RabbitMQMessageQueue>>();
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

            _sendOptions.Setup(x => x.Value.ConfirmReceiptIsActive).Returns(true);

            // Act
            var result = rabbitMQMessageQueue.Send("queue.name", "Data here");

            // Assert
            _channel.Verify(x => x.ConfirmSelect(), Times.Once);
        }

        [Fact]
        public void SendByValidArgumentsWithWorkerConfirmsShouldHitWaitForConfirmsOrDie()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();

            _sendOptions.Setup(x => x.Value.ConfirmReceiptIsActive).Returns(true);

            // Act
            var result = rabbitMQMessageQueue.Send("queue.name", "Data here");

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

        [Fact]
        public void InvokeWithExceptionalResultShouldHitBasicPublish()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();
            var ea = new BasicDeliverEventArgs { Body = Encoding.UTF8.GetBytes("any message") };
            var queueName = "queue.name";

            // Act
            rabbitMQMessageQueue.Invoke(body => throw new Exception(), ea, _channel.Object, queueName);

            // Arrange
            // BasicPublish not virtual.
            _channel.Verify(x => x.CreateBasicProperties(), Times.Once);
        }

        [Fact]
        public void InvokeWithFalseResultShouldHitBasicPublish()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();
            var ea = new BasicDeliverEventArgs { Body = Encoding.UTF8.GetBytes("any message") };
            var queueName = "queue.name";

            // Act
            rabbitMQMessageQueue.Invoke(body => { return false; }, ea, _channel.Object, queueName);

            // Arrange
            // BasicPublish not virtual.
            _channel.Verify(x => x.CreateBasicProperties(), Times.Once);
        }

        [Fact]
        public void InvokeWithTrueResultShouldNotHitBasicPublish()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();
            var ea = new BasicDeliverEventArgs { Body = Encoding.UTF8.GetBytes("any message") };
            var queueName = "queue.name";

            // Act
            rabbitMQMessageQueue.Invoke(body => { return true; }, ea, _channel.Object, queueName);

            // Arrange
            // BasicPublish not virtual.
            _channel.Verify(x => x.CreateBasicProperties(), Times.Never);
        }

        [Fact]
        public void InvokeWithTrueResultShouldAllwaysHitBasicAck()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();

            // Act
            rabbitMQMessageQueue.Invoke(null, new BasicDeliverEventArgs(), _channel.Object, null);

            // Arrange
            _channel.Verify(x => x.BasicAck(It.IsAny<ulong>(), false), Times.Once);
        }

        [Fact]
        public void SendWithValidParamsThanQueueBindArgumentExceptionShouldIngore()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();

            _channel.Setup(x => x.QueueBind("queue.name", It.IsAny<string>(), 
                "queue.name", null))
                .Throws(new ArgumentException());

            // Act
            var result = rabbitMQMessageQueue.Send("queue.name", "message");

            // Arrange
            Assert.True(result);
        }

        [Fact]
        public void ReceiveWithValidParamsThanQueueBindArgumentExceptionShouldIngore()
        {
            // Arrange
            var rabbitMQMessageQueue = GetRabbitMQMessageQueue();

            _channel.Setup(x => x.QueueBind(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), null))
                .Throws(new ArgumentException());

            // Act
            var result = rabbitMQMessageQueue.Receive("queue.name", body => { return true; });

            // Arrange
            Assert.True(result);
        }

        private RabbitMQMessageQueue GetRabbitMQMessageQueue()
        {
            return new RabbitMQMessageQueue(
                _connectionWrapper.Object,
                _jsonConverter.Object,
                _channelFactory.Object,
                _eventingBasicConsumerImpl.Object,
                _sendOptions.Object,
                _logger.Object
            );
        }
    }
}