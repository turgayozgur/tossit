using Tossit.WorkQueue.Job;

namespace Tossit.Job.Api.Jobs
{
    public class BarJob : IJob<BarData>
    {
        public BarData Data { get; set; }

        public string Name => "bar.job.name";
    }
}