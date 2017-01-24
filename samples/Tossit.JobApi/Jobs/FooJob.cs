using Tossit.WorkQueue.Job;

namespace Tossit.JobApi.Jobs
{
    public class FooJob : IJob<FooData>
    {
        public FooData Data { get; set; }

        public string Name => "foo.job.name";
    }
}