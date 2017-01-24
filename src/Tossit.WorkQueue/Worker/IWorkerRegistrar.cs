using System.Threading.Tasks;

namespace Tossit.WorkQueue.Worker
{
    /// <summary>
    /// Worker register interface for registering worker implementations.
    /// </summary>
    internal interface IWorkerRegistrar
    {
        /// <summary>
        /// Register worker for accepting jobs as sync.
        /// </summary>
        /// <typeparam name="TData">Type of data to working on it.</typeparam>
        /// <param name="worker">Worker instance for registering it.</param>
        /// <returns>Returns true if registered successfully, otherwise returns false.</returns>
        bool Register<TData>(IWorker<TData> worker) where TData : class;
        /// <summary>
        /// Register worker for accepting jobs as async.
        /// </summary>
        /// <typeparam name="TData">Type of data to working on it.</typeparam>
        /// <param name="worker">Worker instance for registering it.</param>
        /// <returns>Returns true as Task if registered successfully, otherwise returns false as Task.</returns>
        Task<bool> RegisterAsync<TData>(IWorker<TData> worker) where TData : class;
    }
}