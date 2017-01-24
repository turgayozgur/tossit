using System.Threading.Tasks;

namespace Tossit.WorkQueue.Job
{
    /// <summary>
    /// Interface for job dispatcher implementations.
    /// </summary>
    public interface IJobDispatcher
    {
        /// <summary>
        /// Dispatch job as sync.
        /// </summary>
        /// <typeparam name="TData">Type of data to send to worker.</typeparam>
        /// <param name="job">Job instance to dispatch.</param>
        /// <returns>If job dispatched successfully, returns true, otherwise returns false.</returns>
        /// <exception cref="System.Exception">Throws when job could not be dispatched.</exception>>
        bool Dispatch<TData>(IJob<TData> job) where TData : class;
        /// <summary>
        /// Dispatch job as async.
        /// </summary>
        /// <typeparam name="TData">Type of data to send to worker.</typeparam>
        /// <param name="job">Job instance to dispatch.</param>
        /// <returns>If job dispatched successfully, returns true as Task, otherwise returns false as Task.</returns>
        /// <exception cref="System.Exception">Throws when job could not be dispatched.</exception>>
        Task<bool> DispatchAsync<TData>(IJob<TData> job) where TData : class;
    }
}