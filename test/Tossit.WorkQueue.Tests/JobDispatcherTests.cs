using System;
using Microsoft.Extensions.Logging;
using Moq;
using Tossit.Core;
using Tossit.WorkQueue.Job;
using Xunit;

namespace Tossit.WorkQueue.Tests
{
    public class JobDispatcherTests
    {
        private const string VALID_JOB_NAME = "tossit.unique.for.each.job.jobname";

        private readonly Mock<IJobNameValidator> _jobNameValidator;
        private readonly Mock<IMessageQueue> _messageQueue;
        private readonly Mock<IJsonConverter> _jsonConverter;
        private readonly Mock<ILogger<JobDispatcher>> _logger;

        public JobDispatcherTests()
        {
            _jobNameValidator = new Mock<IJobNameValidator>();
            _jobNameValidator.Setup(x => x.Validate(It.IsAny<string>())).Returns(true);

            _messageQueue = new Mock<IMessageQueue>();
            _messageQueue.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            _jsonConverter = new Mock<IJsonConverter>();
            _jsonConverter.Setup(x => x.Serialize<FooData>(It.IsAny<FooData>())).Returns("{id:1}");

            _logger = new Mock<ILogger<JobDispatcher>>();
        }

        [Fact]
        public void DispatchByValidJobShouldReturnTrue()
        {
            // Arrange
            var jobDispatcher = GetJobDispatcher();

            var job = new FooJob(VALID_JOB_NAME, new FooData());

            // Act
            var result = jobDispatcher.Dispatch(job);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DispatchByAnyJobIfSendReturnsFalseShouldReturnFalse()
        {
            // Arrange
            var jobDispatcher = GetJobDispatcher();

            var job = new FooJob(VALID_JOB_NAME, new FooData());

            _messageQueue.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            // Act
            var result = jobDispatcher.Dispatch(job);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void DispatchByAnyJobIfSendThrowAnyExceptionShouldThrowException()
        {
            // Arrange
            var jobDispatcher = GetJobDispatcher();

            var job = new FooJob(VALID_JOB_NAME, new FooData());

            _messageQueue.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            // Act
            // Assert
            Assert.Throws<Exception>(() => { jobDispatcher.Dispatch(job); });
        }

        [Fact]
        public void DispatchByNullJobShouldThrowArgumentNullException()
        {
            // Arrange
            var jobDispatcher = GetJobDispatcher();

            // Act
            IJob<FooData> job = null;

            // Assert
            Assert.Throws<ArgumentNullException>(nameof(job), () => { jobDispatcher.Dispatch(job); });
        }

        [Fact]
        public void DispatchByNullDataJobShouldThrowArgumentNullException()
        {
            // Arrange
            var jobDispatcher = GetJobDispatcher();

            var job = new FooJob(VALID_JOB_NAME, null);

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(nameof(job.Data), () => { jobDispatcher.Dispatch(job); });
        }

        [Fact]
        public void DispatchByNotNullJobShouldHitJobNameValidatorValidate()
        {
            // Arrange
            var jobDispatcher = GetJobDispatcher();

            var job = new FooJob(VALID_JOB_NAME, new FooData());

            // Act
            var result = jobDispatcher.Dispatch(job);

            // Assert
            _jobNameValidator.Verify(x => x.Validate(VALID_JOB_NAME), Times.Once);
        }

        private JobDispatcher GetJobDispatcher()
        {
            return new JobDispatcher(_jobNameValidator.Object,
                _messageQueue.Object,
                _jsonConverter.Object,
                _logger.Object);
        }

        public class FooJob : IJob<FooData>
        {
            public FooJob(string name, FooData data)
            {
                Name = name;
                Data = data;
            }

            public string Name { get; }

            public FooData Data { get; set; }
        }

        public class FooData
        {

        }
    }
}