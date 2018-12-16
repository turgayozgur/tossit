using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tossit.RabbitMQ;
using Tossit.WorkQueue;

namespace Tossit.Worker.Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add RabbitMQ
            services.AddRabbitMQ();

            // Add Tossit Worker dependencies.
            services.AddTossitWorker();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvc();

            // Use RabbitMQ server
            app.UseRabbitMQServer("amqp://guest:guest@localhost");

            // If this application has worker(s), register it.
            app.UseTossitWorker();
        }
    }
}
