# Tossit (beta) #
[![Build status](https://ci.appveyor.com/api/projects/status/9n48s0tw4h51d0na/branch/master?svg=true)](https://ci.appveyor.com/project/turgayozgur/tossit/branch/master)

Simple, easy to use library to disturbuted job/worker logic. Disturbuted messages handled by built in [RabbitMQ](https://github.com/rabbitmq/rabbitmq-dotnet-client) implementation.
## Installation
You need to install Tossit.RabbitMQ and Tossit.WorkQueue nuget packages.
```
PM> Install-Package Tossit.RabbitMQ
PM> Install-Package Tossit.WorkQueue
```
Use ConfigureServices method on startup to register services.
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add RabbitMQ implementation dependencies.
    services.AddRabbitMQ();

    // Warning!
    // Call only AddTossitJob method or only AddTossitWorker method which one you need. 
    // Call both of them, if current application contains worker and job.

    // Add Tossit Job dependencies.
    services.AddTossitJob();

    // Add Tossit Worker dependencies.
    services.AddTossitWorker(); 
}
```
Than, use configure method to configuring RabbitMQ server and prepare workers.
```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    // Use RabbitMQ server.
    app.UseRabbitMQServer("amqp://guest:guest@localhost");

    // Warning!
    // Call UseTossitWorker method, if current application contains worker.

    // If this application has worker(s), register it.
    app.UseTossitWorker();
}
```
## Job Useage
Create new class to sending to worker(s).
```csharp
public class FooData
{
    public int Id { get; set; }
}
```
Create new job class to dispatch data to worker(s).
```csharp
public class FooJob : IJob<FooData>
{
    public FooData Data { get; set; }

    public string Name => "foo.job.name"; // Job name should be unique for each job.
}
```
Dispatch job using IJobDispatcher.
```csharp
[Route("api/[controller]")]
public class AnyController : Controller
{
    private readonly IJobDispatcher _jobDispatcher;

    public AnyController(IJobDispatcher jobDispatcher)
    {
        _jobDispatcher = jobDispatcher;
    }

    [HttpGet]
    public IActionResult Create()
    {
        // Dispatch job.
        var isDispatched = _jobDispatcher.Dispatch(new FooJob
        {
            Data = new FooData
            {
                Id = 1
            }
        });

        return Ok();
    }
}
```
You can also dispatch a job as async.
```csharp
Task<bool> task = _jobDispatcher.DispatchAsync(new FooJob { Data = new FooData { Id = 1 } });
```
## Worker Useage ##
Create new class to accepting data sent from jobs.
```csharp
public class FooData
{
    public int Id { get; set; }
}
```
Create new worker class to process given data.
```csharp
public class FooWorker : IWorker<FooData>
{
    public string JobName => "foo.job.name"; // Should be same as dispatched job name.

    public bool Work(FooData data)
    {
        // Lets, do whatever you want by data.

        // Return true, if working completed successfully, otherwise return false.
        return true;
    }
}
```
## License
The Tossit is open-sourced software licensed under the [MIT license](https://opensource.org/licenses/MIT).