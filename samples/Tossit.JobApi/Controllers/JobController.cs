using System;
using Microsoft.AspNetCore.Mvc;
using Tossit.JobApi.Jobs;
using Tossit.WorkQueue.Job;

namespace Tossit.JobApi.Controllers
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
            var id = new Random().Next(0, 999);

            // Dispatch job.
            var isDispatched = _jobDispatcher.Dispatch(new FooJob
            {
                Data = new FooData
                {
                    Id = id
                }
            });

            return new ObjectResult(new { Id = id, Message = "Yay! Job dispatched." });
        }
    }
}
