using Xunit;
using System;

namespace Tossit.Core.Tests
{
    public class TimeLoopTests
    {
        [Fact]
        public void StartNewWithValidActionAndPeriodShouldReturnGuid()
        {
            // Arrange
            var timeLoop = new TimeLoop();

            // Act
            var result = timeLoop.StartNew(id => { }, 10);

            // Assert
            Assert.Equal(typeof(Guid), result.GetType());
        }

        [Fact]
        public void StartNewWithNullActionShoultThrowArgumentNullException()
        {
            // Arrange
            var timeLoop = new TimeLoop();

            // Act
            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => { timeLoop.StartNew(null, 10); });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void StartNewWithZeroPeriodShoultThrowArgumentException(int period)
        {
            // Arrange
            var timeLoop = new TimeLoop();

            // Act
            // Assert
            Assert.Throws(typeof(ArgumentException), () => { timeLoop.StartNew(id => { }, period); });
        }

        [Fact]
        public void StopWithTimerShouldReturnTrue()
        {
            // Arrange
            var timeLoop = new TimeLoop();

            // Act
            var loopId = timeLoop.StartNew(id => { }, 10);
            var result = timeLoop.Stop(loopId);

            // Assert
            Assert.Equal(true, result);
        }

        [Fact]
        public void StopWithoutTimerShouldReturnFalse()
        {
            // Arrange
            var timeLoop = new TimeLoop();

            // Act
            var result = timeLoop.Stop(Guid.Empty);

            // Assert
            Assert.Equal(false, result);
        }
    }
}