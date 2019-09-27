# Tossit #
[![Latest version](https://img.shields.io/nuget/v/Tossit.RabbitMQ.svg)](https://www.nuget.org/packages/Tossit.RabbitMQ)
[![Build Status](https://travis-ci.org/turgayozgur/tossit.svg?branch=master)](https://travis-ci.org/turgayozgur/tossit)
[![Build status](https://ci.appveyor.com/api/projects/status/whuoamx1tb19jbn6/branch/master?svg=true)](https://ci.appveyor.com/project/turgayozgur/tossit/branch/master)
![](https://github.com/turgayozgur/tossit/workflows/Main/badge.svg)
[![codecov](https://codecov.io/gh/turgayozgur/tossit/branch/master/graph/badge.svg)](https://codecov.io/gh/turgayozgur/tossit)
[![Gitter](https://img.shields.io/gitter/room/nwjs/nw.js.svg)](https://gitter.im/tossitchat/Lobby)

Simple, easy to use library for distributed job/worker logic. Distributed messages handled by built in [RabbitMQ](https://github.com/rabbitmq/rabbitmq-dotnet-client) implementation.
## Highlights ##
* Super easy way to use RabbitMQ .net client for job/worker logic.
* Connection and channel management.
* Failure management.
* Send and receive data that auto converted to your object types.
* Recovery functionality. Do not worry about connection loses.
## Installation ##
You need to install [Tossit.RabbitMQ](https://www.nuget.org/packages/Tossit.RabbitMQ) nuget package.
```
PM> Install-Package Tossit.RabbitMQ
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
Then, use configure method to configuring RabbitMQ server and prepare workers.
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
## Job Usage ##
Create a new class to sending to worker(s).
```csharp
public class FooData
{
    public int Id { get; set; }
}
```
Create a new job class to dispatch data to worker(s).
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
### Send Options ###
* **WaitToRetrySeconds:** Time as second for wait to retry when job rejects or throws an error. Should be greater then zero. Default 30 seconds.

* **ConfirmReceiptIsActive:** Set true if u want to wait to see the data received successfully from a worker until timeout. Otherwise can be false. It is highly recommended to be true. Default: true.

* **ConfirmReceiptTimeoutSeconds**: Wait until a dispatched data have been confirmed. Default 10 seconds.
```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...

    // Add Tossit Job dependencies with options.
    services.AddTossitJob(sendOptions => {
        sendOptions.WaitToRetrySeconds = 30;
        sendOptions.ConfirmReceiptIsActive = true;
        sendOptions.ConfirmReceiptTimeoutSeconds = 10;
    });

    ...
}
```
## Worker Usage ##
Create new class to accept the data sent from jobs.
```csharp
public class FooData
{
    public int Id { get; set; }
}
```
Create a new worker class to process given data.
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
## License ##
The Tossit is open-sourced software licensed under the [MIT license](https://opensource.org/licenses/MIT).
