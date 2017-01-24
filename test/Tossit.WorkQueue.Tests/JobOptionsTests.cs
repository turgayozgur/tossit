using Xunit;
using Tossit.WorkQueue.Job;

namespace Tossit.WorkQueue.Tests
{
    public class JobOptionsTests
    {
        [Fact]
        public void GetPublisherConfirmsTimeoutWithoutTimeoutParamShouldReturnTimespan()
        {
            // Arrange
            var jobOptions = new JobOptions();

            // Act
            var result = jobOptions.WorkerConfirmsTimeoutSeconds;

            // Assert
            Assert.True(result == 10);
        }

        [Fact]
        public void GetPublisherConfirmsTimeoutWithTimeoutParamShouldReturnTimespan()
        {
            // Arrange
            var jobOptions = new JobOptions{
                WorkerConfirmsTimeoutSeconds = 30
            };

            // Act
            var result = jobOptions.WorkerConfirmsTimeoutSeconds;

            // Assert
            Assert.True(result == 30);
        }
    }
}