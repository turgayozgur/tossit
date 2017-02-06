using System;
using Tossit.Core;
using Xunit;

namespace Tossit.WorkQueue.Tests
{
    public class SendOptionsTests
    {
        [Fact]
        public void GetPublisherConfirmsTimeoutWithoutTimeoutParamShouldReturnTimespan()
        {
            // Arrange
            var sendOptions = new SendOptions();

            // Act
            var result = sendOptions.ConfirmReceiptTimeoutSeconds;

            // Assert
            Assert.True(result == 10);
        }

        [Fact]
        public void GetPublisherConfirmsTimeoutWithTimeoutParamShouldReturnTimespan()
        {
            // Arrange
            var sendOptions = new SendOptions
            {
                ConfirmReceiptTimeoutSeconds = 30
            };

            // Act
            var result = sendOptions.ConfirmReceiptTimeoutSeconds;

            // Assert
            Assert.True(result == 30);
        }

        [Fact]
        public void GetWaitToRetryWithoutSetShouldReturnDefaultValue()
        {
            // Arrange
            var sendOptions = new SendOptions();

            // Act
            var result = sendOptions.WaitToRetrySeconds;

            // Assert
            Assert.True(result == 30);
        }

        [Theory]
        [InlineData(999)]
        [InlineData(1)]
        public void GetWaitToRetryWitSetShouldReturnGivenValue(int second)
        {
            // Arrange
            var sendOptions = new SendOptions
            {
                WaitToRetrySeconds = second
            };

            // Act
            var result = sendOptions.WaitToRetrySeconds;

            // Assert
            Assert.True(result == second);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SetWaitToRetryWithLowerThanOneShouldThrowArgumentException(int second)
        {
            // Arrange
            var sendOptions = new SendOptions();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => { sendOptions.WaitToRetrySeconds = second; });
        }
    }
}