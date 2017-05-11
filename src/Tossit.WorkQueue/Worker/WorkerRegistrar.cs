using System;
#if !net451
using Microsoft.Extensions.Logging;
#endif
using Tossit.Core;
using Tossit.WorkQueue.Job;

namespace Tossit.WorkQueue.Worker
{
    /// <summary>
    /// Worker register implementation for registering worker.
    /// </summary>
    internal class WorkerRegistrar : IWorkerRegistrar
    {
        /// <summary>
        /// JobNameValidator field.
        /// </summary>
        private readonly IJobNameValidator _jobNameValidator;
        /// <summary>
        /// JsonConverter field.
        /// </summary>
        private readonly IJsonConverter _jsonConverter;
        /// <summary>
        /// MessageQueue field.
        /// </summary>
        private readonly IMessageQueue _messageQueue;
#if !net451
        /// <summary>
        /// Logger field.
        /// </summary>
        private readonly ILogger<WorkerRegistrar> _logger;
#endif

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="jobNameValidator">IJobNameValidator</param>
        /// <param name="jsonConverter">IJsonConverter</param>
        /// <param name="messageQueue">IMessageQueue</param>
#if !net451
        /// <param name="logger">ILogger{WorkerRegistrar}</param>
#endif
        public WorkerRegistrar(IJobNameValidator jobNameValidator,
            IJsonConverter jsonConverter,
            IMessageQueue messageQueue
#if !net451            
            ,ILogger<WorkerRegistrar> logger
#endif            
            )
        {
            _jobNameValidator = jobNameValidator;
            _jsonConverter = jsonConverter;
            _messageQueue = messageQueue;
#if !net451            
            _logger = logger;
#endif
        }

        /// <summary>
        /// Register worker for accepting jobs as sync.
        /// </summary>
        /// <typeparam name="TData">Type of data to working on it.</typeparam>
        /// <param name="worker">Worker instance for registering it.</param>
        /// <returns>Returns true if registered successfully, otherwise returns false.</returns>
        public bool Register<TData>(IWorker<TData> worker) where TData : class
        {
            this.ValidateWorker(worker);

            try
            {
                // Receive
                var result = _messageQueue.Receive(
                        worker.JobName, 
                        body => worker.Work(_jsonConverter.Deserialize<TData>(body)));

                // Log, if could not be registerd.
                if (!result)
                {
                    var message = $"Worker {worker.GetType().FullName} could not be registered.";
#if !net451
                    _logger.LogError(message);
#endif
                    return false;
                }
            }
            catch (Exception ex)
            {
#if !net451                
                _logger.LogError(
                    new EventId(), 
                    ex, 
                    $"Worker {worker.GetType().FullName} registration failed. {ex.Message}");
#endif

                throw ex;
            }

#if !net451
            _logger.LogInformation($"Worker {worker.GetType().FullName} successfully registered.");
#endif

            return true;
        }

        /// <summary>
        /// Validate given worker.
        /// </summary>
        /// <typeparam name="TData">Type of worker data.</typeparam>
        /// <param name="worker">Instance of worker.</param>
        /// <exception cref="ArgumentNullException">Throws when worker is null.</exception>
        private void ValidateWorker<TData>(IWorker<TData> worker) where TData : class
        {
            if (worker == null)
            {
                throw new ArgumentNullException(nameof(worker));
            }

            _jobNameValidator.Validate(worker.JobName);
        }
    }
}