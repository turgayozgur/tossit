using System;
using Tossit.WorkQueue.Worker;

namespace Tossit.WorkerApi.Workers
{
    public class FooWorker : IWorker<FooData>
    {
        public string JobName => "foo.job.name";

        public bool Work(FooData data)
        {
            // Lets, do whatever u want by data.
            Console.WriteLine(data.Id);

            // Return true, if working completed successfully.
            return true;
        }
    }
}