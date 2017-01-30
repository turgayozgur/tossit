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
            var sendOptions = new SendOptions{
                ConfirmReceiptTimeoutSeconds = 30
            };

            // Act
            var result = sendOptions.ConfirmReceiptTimeoutSeconds;

            // Assert
            Assert.True(result == 30);
        }
    }
}