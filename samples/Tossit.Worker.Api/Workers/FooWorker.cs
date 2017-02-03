using Microsoft.Extensions.Logging;
using Tossit.WorkQueue.Worker;

namespace Tossit.Worker.Api.Workers
{
    public class FooWorker : IWorker<FooData>
    {
        public string JobName => "foo.job.name";

        private readonly ILogger<FooWorker> _logger;

        public FooWorker(ILogger<FooWorker> logger)
        {
            _logger = logger;
        }

        public bool Work(FooData data)
        {
            // Lets, do whatever u want by data.
            _logger.LogInformation($"Yep! It worked. id: {data.Id}");

            // Return true, if working completed successfully.
            return true;
        }
    }
}