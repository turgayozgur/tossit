#if !net451
using System;
using Microsoft.Extensions.DependencyInjection;
using Tossit.Core;
using Tossit.WorkQueue.Job;
using Tossit.WorkQueue.Worker;

namespace Tossit.WorkQueue
{
    /// <summary>
    /// Tossit.WorkQueue.ServiceCollectionExtensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register tossit job dependencies.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configure">Action{Options}</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void AddTossitJob(this IServiceCollection services, Action<SendOptions> configure = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Tossit.Core
            services.AddTossitCore();

            // Tossit.WorkQueue.Job
            services.Configure(configure ?? (options => {}));
            services.AddScoped<IJobNameValidator, JobNameValidator>();
            services.AddScoped<IJobDispatcher, JobDispatcher>();
        }

        /// <summary>
        /// Register tossit worker dependencies.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <exception cref="ArgumentNullException">Throws when services is null.</exception>
        public static void AddTossitWorker(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Tossit.Core
            services.AddTossitCore();

            // Tossit.WorkQueue.Worker
            services.AddScoped<IJobNameValidator, JobNameValidator>();
            services.AddSingleton<IWorkerRegistrar, WorkerRegistrar>();

            // Add workers as singleton.
            var workerTypes = new ReflectionHelper(new DependencyContextProxy())
                .GetTypesThatImplementedByInterface(typeof(IWorker<>));

            foreach (var workerType in workerTypes)
            {
                services.AddSingleton(typeof(IConsumer), workerType);
            }
        }
    }
}
#endif