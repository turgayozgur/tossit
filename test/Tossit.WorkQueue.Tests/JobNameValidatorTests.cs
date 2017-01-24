using System;
using Tossit.WorkQueue.Job;
using Xunit;

namespace Tossit.WorkQueue.Tests
{
    public class JobNameValidatorTests
    {
        private const string VALID_JOB_NAME = "tossit.job.name";
        private const string INVALID_JOB_NAME = "InvalidJobName";

        [Fact]
        public void ValidateByValidJobNameShouldReturnTrue()
        {
            // Arrange
            var jobNameValidator = new JobNameValidator();
            var jobName = VALID_JOB_NAME;

            // Act
            var result = jobNameValidator.Validate(jobName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateByNullJobNameShouldThrowArgumentNullException()
        {
            // Arrange
            var jobNameValidator = new JobNameValidator();
            string jobName = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(nameof(jobName), () => { jobNameValidator.Validate(jobName); });
        }

        [Fact]
        public void ValidateByInvalidJobNameShouldThrowException()
        {
            // Arrange
            var jobNameValidator = new JobNameValidator();
            var jobName = INVALID_JOB_NAME;

            // Act
            // Assert
            Assert.Throws(typeof(Exception), () => { jobNameValidator.Validate(jobName); });
        }
    }
}