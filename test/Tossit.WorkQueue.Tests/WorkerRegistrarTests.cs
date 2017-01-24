using System;
using Xunit;
using Moq;
using Tossit.Core;
using Microsoft.Extensions.Logging;
using Tossit.WorkQueue.Job;
using Tossit.WorkQueue.Worker;
using System.Threading.Tasks;

namespace Tossit.WorkQueue.Tests
{
    public class WorkerRegistrarTests
    {
        private const string VALID_JOB_NAME = "tossit.unique.for.each.worker.jobname";

        private readonly Mock<IJobNameValidator> _jobNameValidator;        
        private readonly Mock<IJsonConverter> _jsonConverter;
        private readonly Mock<IMessageQueue> _messageQueue;  
        private readonly Mock<ILogger<WorkerRegistrar>> _logger;

        public WorkerRegistrarTests()
        {
            _jobNameValidator = new Mock<IJobNameValidator>();
            _jobNameValidator.Setup(x => x.Validate(It.IsAny<string>())).Returns(true);

            _jsonConverter = new Mock<IJsonConverter>();
            _jsonConverter.Setup(x => x.Deserialize<FooData>(It.IsAny<string>())).Returns(new FooData());

            _messageQueue = new Mock<IMessageQueue>();
            _messageQueue.Setup(x => x.Receive(It.IsAny<string>(), It.IsAny<Func<string, bool>>()))
                .Returns(true);

            _logger = new Mock<ILogger<WorkerRegistrar>>();
        }

        [Fact]
        public void RegisterByValidWorkerShouldReturnTrue()
        {
            // Arrange
            var workerRegistrar = GetWorkerRegistrar();

            var worker = new FooWorker(VALID_JOB_NAME);

            // Act
            var result = workerRegistrar.Register(worker);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void RegisterAsyncByValidWorkerShouldReturnTrue()
        {
            // Arrange
            var workerRegistrar = GetWorkerRegistrar();

            var worker = new FooWorker(VALID_JOB_NAME);

            // Act
            var task = Task.Run(() => workerRegistrar.RegisterAsync(worker));
            task.Wait();

            // Assert
            Assert.True(task.Result);
        }

        [Fact]
        public void RegisterByAnyWorkerIfReceiveReturnsFalseShouldReturnFalse()
        {
            // Arrange
            var workerRegistrar = GetWorkerRegistrar();

            var worker = new FooWorker(VALID_JOB_NAME);

            _messageQueue.Setup(x => x.Receive(It.IsAny<string>(), It.IsAny<Func<string, bool>>()))
                .Returns(false);

            // Act
            var result = workerRegistrar.Register(worker);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void RegisterByAnyWorkerIfSendThrowAnyExceptionShouldThrowException()
        {
            // Arrange
            var workerRegistrar = GetWorkerRegistrar();

            var worker = new FooWorker(VALID_JOB_NAME);

            _messageQueue.Setup(x => x.Receive(It.IsAny<string>(), It.IsAny<Func<string, bool>>()))
                .Throws<Exception>();

            // Act
            // Assert
            Assert.Throws<Exception>(() => { workerRegistrar.Register(worker); });
        }

        [Fact]
        public void RegisterByNullWorkerShouldThrowArgumentNullException()
        {
            // Arrange
            var workerRegistrar = GetWorkerRegistrar();
            
            // Act
            IWorker<FooData> worker = null;
            
            // Assert
            Assert.Throws<ArgumentNullException>(nameof(worker), () => { workerRegistrar.Register<FooData>(worker); });
        }

        [Fact]
        public void RegisterByNotNullWorkerShouldHitJobNameValidatorValidate()
        {
            // Arrange
            var workerRegistrar = GetWorkerRegistrar();

            var worker = new FooWorker(VALID_JOB_NAME);

            // Act
            var result = workerRegistrar.Register(worker);

            // Assert
            _jobNameValidator.Verify(x => x.Validate(VALID_JOB_NAME), Times.Once);
        }

        private WorkerRegistrar GetWorkerRegistrar()
        {
            return new WorkerRegistrar(
                _jobNameValidator.Object,
                _jsonConverter.Object,
                _messageQueue.Object,
                _logger.Object);
        }

        public class FooWorker : IWorker<FooData>
        {
            public FooWorker(string jobName)
            {
                JobName = jobName;
            }

            public string JobName { get; }

            public bool Work(FooData data)
            {
                return true;
            }
        }

        public class FooData
        {

        }
    }
}