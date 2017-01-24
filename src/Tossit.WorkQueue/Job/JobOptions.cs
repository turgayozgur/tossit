namespace Tossit.WorkQueue.Job
{
    /// <summary>
    /// Job options.
    /// </summary>
    public class JobOptions
    {
        /// <summary>
        /// Default worker confirms timeout as second. Value: 10 sec.
        /// RabbitMQ uses publisher confirms to guarantee that data has been sent.
        /// </summary>
        private const int DEAFULT_WORKER_CONFIRMS_TIMEOUT = 10;

        /// <summary>
        /// WorkerConfirmsTimeoutSeconds field.
        /// </summary>
        private int _workerConfirmsTimeoutSeconds;

        /// <summary>
        /// Default 10 seconds. Wait until a dispatched job have been confirmed from any worker.
        /// Returns default if not specified.
        /// </summary>
        public virtual int WorkerConfirmsTimeoutSeconds
        {
            get
            {
                return _workerConfirmsTimeoutSeconds > 0 ? _workerConfirmsTimeoutSeconds : DEAFULT_WORKER_CONFIRMS_TIMEOUT;
            }
            set
            {
                _workerConfirmsTimeoutSeconds = value;
            }
        }

        /// <summary>
        /// True if u want to wait to successfully worked message from worker until timeout, otherwise should be false.
        /// </summary>
        public virtual bool WorkerConfirmsIsActive { get; set; }
    }
}