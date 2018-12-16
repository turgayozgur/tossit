using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tossit.RabbitMQ;
using Tossit.WorkQueue;

namespace Tossit.Job.Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add RabbitMQ implementation dependencies.
            services.AddRabbitMQ();

            // Add Tossit Job dependencies.
            services.AddTossitJob(sendOptions => {
                sendOptions.WaitToRetrySeconds = 30;
                sendOptions.ConfirmReceiptIsActive = true;
                sendOptions.ConfirmReceiptTimeoutSeconds = 10;
            });            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvc();

            // Use RabbitMQ server
            app.UseRabbitMQServer("amqp://guest:guest@localhost"); 
        }
    }
}
