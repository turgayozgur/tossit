namespace Tossit.WorkQueue.Job
{
    /// <summary>
    /// Interface for job implementations.
    /// Jobs are dispatched to be processed by workers.
    /// </summary>
    /// <typeparam name="TData">Type of data to send to workers.</typeparam>
    public interface IJob<TData> where TData : class
    {
        /// <summary>
        /// Data to send to workers.
        /// </summary>
        TData Data { get; set; }
        /// <summary>
        /// Name of job.
        /// Name should be given by dot notation and lower case. e.g: tossit.anything.name
        /// </summary>
        string Name { get; }
    }
}
