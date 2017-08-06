using System;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Xunit;

namespace Tossit.RabbitMQ.Tests
{
    public class ConsumerChannelRecoveryTests : IClassFixture<ConsumerChannelRecoveryTestsFixture>
    {
        private readonly ConsumerChannelRecoveryTestsFixture _fixture;

        public ConsumerChannelRecoveryTests(ConsumerChannelRecoveryTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void AttemptWhenConnectionClosedShouldNotHitRecoveryMethod()
        {
            // Arrange
            var consumerChannelRecovery = _fixture.GetTestObject();
            var isActionCalled = false;
            _fixture.Connection.Setup(x => x.IsOpen).Returns(false);

            // Act
            consumerChannelRecovery.Attempt(_fixture.Channel.Object, channel => { isActionCalled = true; });

            // Assert
            Assert.Equal(false, isActionCalled);
        }

        [Fact]
        public void AttemptWhenChannelClosedShouldNotHitRecoveryMethod()
        {
            // Arrange
            var consumerChannelRecovery = _fixture.GetTestObject();
            var isActionCalled = false;
            _fixture.Connection.Setup(x => x.IsOpen).Returns(true);
            _fixture.Channel.Setup(x => x.IsOpen).Returns(false);

            // Act
            consumerChannelRecovery.Attempt(_fixture.Channel.Object, channel => { isActionCalled = true; });

            // Assert
            Assert.Equal(false, isActionCalled);
        }

        [Fact]
        public void AttemptWhenChannelAndConnectionAliveShouldHitRecoveryMethod()
        {
            // Arrange
            var consumerChannelRecovery = _fixture.GetTestObject();
            var isActionCalled = false;
            _fixture.Connection.Setup(x => x.IsOpen).Returns(true);
            _fixture.Channel.Setup(x => x.IsOpen).Returns(true);

            // Act
            consumerChannelRecovery.Attempt(_fixture.Channel.Object, channel => { isActionCalled = true; });

            // Assert
            Assert.Equal(true, isActionCalled);
        }
    }

    public class ConsumerChannelRecoveryTestsFixture : IDisposable
    {
        public Mock<IConnectionWrapper> ConnectionWrapper { get; }
        public Mock<ILogger<ConsumerChannelRecovery>> Logger { get; }
        public Mock<ITimeLoop> TimeLoop { get; }
        public Mock<IModel> Channel { get; }
        public Mock<IConnection> Connection { get; }

        public ConsumerChannelRecoveryTestsFixture()
        {
            // Mocks.
            ConnectionWrapper = new Mock<IConnectionWrapper>();
            Logger = new Mock<ILogger<ConsumerChannelRecovery>>();
            TimeLoop = new Mock<ITimeLoop>();
            Channel = new Mock<IModel>();
            Connection = new Mock<IConnection>();
            
            // Setups.
            ConnectionWrapper.Setup(x => x.ConsumerConnection).Returns(Connection.Object);
        }

        public ConsumerChannelRecovery GetTestObject()
        {
            return new ConsumerChannelRecovery(ConnectionWrapper.Object, Logger.Object, TimeLoop.Object);
        }

        public void Dispose()
        {
            // Cleanup here.
        }
    }
}