using System;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Tossit.Core;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// Tossit.RabbitMQ.ServiceCollectionExtensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register tossit RabbitMQ dependencies.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void AddRabbitMQ(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // RabbitMQ
            services.AddScoped<IConnectionFactory, ConnectionFactory>();

            // Tossit.RabbitMQ
            services.AddSingleton<IMessageQueue, RabbitMQMessageQueue>();

            services.AddSingleton<IConnectionWrapper, ConnectionWrapper>();
            services.AddSingleton<IChannelFactory, ConsumerChannelFactory>();
            services.AddSingleton<IChannelRecovery, ConsumerChannelRecovery>();

            services.AddScoped<IEventingBasicConsumerImpl, EventingBasicConsumerImpl>();
        }
    }
}