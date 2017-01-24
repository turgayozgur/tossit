using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Tossit.RabbitMQ
{
    /// <summary>
    /// Tossit.RabbitMQ.ApplicationBuilderExtensions
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Use RabbitMQ server.
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="uri">RabbitMQ server uri. e.g: amqp://username:pass@localhost</param>
        /// <exception cref="ArgumentNullException">Throws when app is null.</exception>
        public static void UseRabbitMQServer(this IApplicationBuilder app, string uri = null)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            var services = app.ApplicationServices;

            // Configure dispose to channels and connections when application stopped.
            var connectionWrapper = services.GetRequiredService<IConnectionWrapper>();
            var channelFactory = services.GetRequiredService<IChannelFactory>();

            if (connectionWrapper == null || channelFactory == null)
            {
                throw new InvalidOperationException("Unable to find the required services.");
            }

            var lifetime = services.GetRequiredService<IApplicationLifetime>();

            // Register channel and connections dispose to ApplicationStopped event.
            lifetime.ApplicationStopped.Register(() => channelFactory.Dispose());
            lifetime.ApplicationStopped.Register(() => connectionWrapper.Dispose());

            // Set server uri, if specified.
            if (!string.IsNullOrWhiteSpace(uri))
            {
                var connectionFactory = services.GetRequiredService<IConnectionFactory>();
                ((ConnectionFactory)connectionFactory).SetUri(new Uri(uri));
            }
        }
    }
}