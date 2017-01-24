using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Xunit;

namespace Tossit.RabbitMQ.Tests
{
    public class ConsumerInvokerTests
    {
        private readonly Mock<IModel> _model;
        private readonly Mock<BasicDeliverEventArgs> _basicDeliverEventArgs;

        public ConsumerInvokerTests()
        {
            _model = new Mock<IModel>();
            _basicDeliverEventArgs = new Mock<BasicDeliverEventArgs>();
        }

        [Fact]
        public void InvokeByTrueResultFuncShouldHitBasicAck()
        {
            // Arrange
            var consumerInvoker = new ConsumerInvoker();

            // Act
            consumerInvoker.Invoke((str) => { return true; }, _basicDeliverEventArgs.Object, _model.Object);

            // Assert
            _model.Verify(x => x.BasicAck(It.IsAny<ulong>(), false), Times.Once);
        }

        [Fact]
        public void InvokeByFalseResultFuncShouldHitBasicNack()
        {
            // Arrange
            var consumerInvoker = new ConsumerInvoker();

            // Act
            consumerInvoker.Invoke((str) => { return false; }, _basicDeliverEventArgs.Object, _model.Object);

            // Assert
            _model.Verify(x => x.BasicNack(It.IsAny<ulong>(), false, true), Times.Once);
        }
    }
}