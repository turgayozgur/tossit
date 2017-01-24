using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tossit.Core
{
    /// <summary>
    /// Tossit.Core.ServiceCollectionExtensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register tossit core dependencies.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <exception cref="ArgumentNullException">Throws when services is null.</exception>
        public static void AddTossitCore(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IJsonConverter, JsonConverter>();
            services.AddSingleton<IReflectionHelper, ReflectionHelper>();
        }
    }
}