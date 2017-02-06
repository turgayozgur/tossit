using System;
using Tossit.WorkQueue.Worker;

namespace Tossit.Worker.Api.Workers
{
    public class BarWorker : IWorker<BarData>
    {
        public string JobName => "bar.job.name";

        public bool Work(BarData data)
        {
            // If worker throws any exception, tossit will try to process this job again after 30 seconds as default.
            throw new Exception($"Oops:( {data.Name} needs your help...");
        }
    }
}