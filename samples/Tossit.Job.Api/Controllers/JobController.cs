using System;
using Microsoft.AspNetCore.Mvc;
using Tossit.Job.Api.Jobs;
using Tossit.WorkQueue.Job;

namespace Tossit.Job.Api.Controllers
{
    [Route("api/[controller]")]
    public class JobController : Controller
    {
        private readonly IJobDispatcher _jobDispatcher;

        public JobController(IJobDispatcher jobDispatcher)
        {
            _jobDispatcher = jobDispatcher;
        }

        /// <summary>
        /// Get api/job
        /// Create new FooJob job.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            var random = new Random().Next(0, 999);

            // Dispatch foo job.
            _jobDispatcher.Dispatch(new FooJob
            {
                Data = new FooData
                {
                    Id = random
                }
            });

            // Dispatch bar job.
            _jobDispatcher.Dispatch(new BarJob
            {
                Data = new BarData
                {
                    Name = $"bar:{random}"
                }
            });

            return new ObjectResult(new
            {
                Id = random,
                Message = "Yay! Foo and bar jobs dispatched."
            });
        }
    }
}
