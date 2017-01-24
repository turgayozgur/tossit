using Tossit.Core;

namespace Tossit.WorkQueue.Worker
{
    /// <summary>
    /// Worker interface for worker implementations.
    /// Workers subscribes to jobs and notified when jobs dispatched with same job name.
    /// </summary>
    /// <typeparam name="TData">Type of data to working on it.</typeparam>
    public interface IWorker<in TData> : IConsumer where TData : class
    {
        /// <summary>
        /// Work on dispatched job's data's.
        /// </summary>
        /// <param name="data">Dispatched data to working on it.</param>
        /// <returns>Returns true if work successfully done, otherwise returns false.</returns>
        bool Work(TData data);
        /// <summary>
        /// Name of work to be worked on.
        /// </summary>
        string JobName { get; }
    }
}