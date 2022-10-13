# Distributed-Circuit-Breaker

## Description
This repo is just a proof of concept of distributed circuit breaker. 

Distributed Circuit Breaker can be helpful if you need to share a circuit breaker instance across multiple services (or multiple instances of the same service), which implies that circuit breaker's state is kept in some external storage (Redis, Mongo and so on...) as opposed to service instance memory. 

## Context

Circuit breaker can be thought as a resilient wrapper of the an external system state which allows you to prevent overwheming external system with redundant request and fail fast or fallback when the external system is not healthy.

Goto solution for applying circuit breakers is lovely [Polly](https://github.com/App-vNext/Polly) library. It allows you to nicely create/reuse circuit breakers that can be shared across the application code. But Circuit Breaker of Polly is in-memory, its state is available only within one application instance.

What if we want to share the same circuit breaker with other services in order for them to be also aware of that the external system unhealthiness ? 

## Circuit breaker simple usesage scenario (without this library)

Let's say that we have some order service that can place order.

Here is the flow
- Send 'Place order' command to OrderService
- OrderService charge user for order, by communicating with PatementGateway
- OrderService asks ShipmentService to ship the order

![order-service-with-circuit-breaker](https://raw.githubusercontent.com/shamil-sadigov/Distributed-Circuit-Breaker/main/docs/images/Small%20ones/order-service-with-circuit-breakerjpg.jpg)

OrderService uses in-memory singleton Circuit Breaker when it deals with ShipmentService(for example [Polly Circuit Breaker](https://github.com/App-vNext/Polly#circuit-breaker)) .

### Example detailed flow:
- At some sunny day (usually Friday the 13th), ShipmentService decided to break down, become unhealthy, unavailbe, just died!
- 'Place order' command arrived to OrderService
- After a 5 failed attempts to ship order OrderService will figure out that ShipmentService is unhealthy, and probably there is no any point to try to ship order at least the next 10 seconds, because ShipmentService is for sure will be unhealthy. 
- As a result, CircuitBreaker of ShipmentService is be broken, and moved to **Open state** for configured **duration of break** (10 seconds) .
- Withing **duraion of break** (10 seconds) no any request to ShipmentService will be allowed in OrderService
- Once **duration of break** is elapsed, CircuitBreaker moves to **HalfOpen state** which allow to talk to ShipmentService in order to find out wether ShipmentService recovered or not.
- If ShipmentService is recovered (e.g responded successfully), then CircuitBreaker will switch to **Closed** state, otherwise it will return to **Open state** and again will prevent any subsequent request so ShipmentService withing **duration of break** (10 seconds) 

![order-service-with-circuit-breaker-in-open-state](https://raw.githubusercontent.com/shamil-sadigov/Distributed-Circuit-Breaker/main/docs/images/Small%20ones/order-service-with-circuit-breaker-in-open-state.jpg)


### Problem: in distributed system


// TODO: Add reference for circuit breaker




But Critical-Log-saver is not aware yet about unhealthy state of Log Storage, which means that Critical-Log-saver also have to make the same amount of failed attempts in order to realize that Log Storage is unhealthy and turn CircuitBreaker to Open state. But why do these redundant requests to Log Storage ?



PS: Well, not the best example, but acceptable for conveying a concept

## Solution

Here we introduce stateful Circuit Breaker that is shared among Trace-log-saver and Critical-log-saver. 

It means that:
- If Log Storage goes down.
- Trace-log-saver will notice it after several failed attempts when trying to send logs to Log Storage
- As a resultm Trace-log-saver switches CircuitBreaker to Open state.

And since CircuitBreaker state is globally available, Critical-Log-saver can access this CircuitBreaker and be aware of Log Storage unhealthy state. No need for redundant requests, unnecessary resource consumption and other things!

![stateful-circuit-breakers](https://github.com/shamil-sadigov/Distributed-Circuit-Breaker/blob/main/docs/images/Small%20ones/stateful-circuit-breaker.jpg)

Especially it can come handy when replicating servies sharing the same Circuit Breaker

![replicated-services](https://github.com/shamil-sadigov/Distributed-Circuit-Breaker/blob/main/docs/images/Small%20ones/replication.jpg)


So, this repo implements this solution.

## Solution 2

Another approach is to place Log Storage behind a Proxy service (aka [Sidecar pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/sidecar)) that can keep Circuit Breaker. But this repo doesn't implement this solution

![proxy-circuit-breaker](https://github.com/shamil-sadigov/Distributed-Circuit-Breaker/blob/main/docs/images/Small%20ones/circuit-breaker-via-side-card.jpg)


## How to use

First create circuit breaker option

```cs
// Circuit breaker for remote log storage
public sealed class LogStorageCircuitBreakerOptions : CircuitBreakerOptions
{
    public LogStorageCircuitBreakerOptions()
    {
        // Exception and results that circuit breaker should handle
        HandleException<EventStoreConnectionException>(x => x.FailureReason == LogStorageFailureReason.Overwhelmed);
        HandleException<EventStoreConnectionException>(x => x.FailureReason == LogStorageFailureReason.Unavailable);
        
        HandleResult<SavedLogResult>(x => !x.IsLogSaved);
    }

    // circuit breaker name that should be unique across all circuit breakers
    public override string Name => "LogStorage";

    // number of failed allowed before switching circuit breaker to Open state
    public override int FailureAllowedBeforeBreaking => 5;

    // how long it will take to switch from Open to HalfOpen state
    public override TimeSpan DurationOfBreak => TimeSpan.FromSeconds(5);
}
```

Then configure distributed circuit breaker by registering previous option in ServiceCollection and specify storage (Mongo or MSSQL) 

Example with Mongo storage
```cs
builder.Services.AddDistributedCircuitBreaker(ops =>
{
    ops.UseMongo(x =>
    {
        // required
        x.ConnectionString = "mongodb://localhost:27017";

        // optional
        x.CollectionName = "CircuitBreakers"; // default value
        x.DatabaseName = "DistributedCircuitBreaker"; // default value
    })
    .AddCircuitBreaker<LogStorageCircuitBreakerOptions>();
});
```

Example with SqlServer
```cs
builder.Services.AddDistributedCircuitBreaker(ops =>
{
    ops.UseSqlServer(x =>
    {
        // required
        x.ConnectionString = "...";
    })
    .AddCircuitBreaker<LogStorageCircuitBreakerOptions>();
});
```

You can configure as many circuit breaker options as you want
```cs
builder.Services.AddDistributedCircuitBreaker(ops =>
{
    ops.UseSqlServer(x =>
        {
            // required
            x.ConnectionString = "...";
        })
        .AddCircuitBreaker<LogStorageCircuitBreakerOptions>();
        .AddCircuitBreaker<NotificationServiceCircuitBreakerOptions>();
        .AddCircuitBreaker<AnotherExternalServiceCircuitBreakerOptions>();
});
```

Then just inject circuit breaker in constructor by specifying CircuitBreakerOptions that was registered on previous step

```cs
public class CriticalLogsController : ControllerBase
{
    ...
    private readonly ICircuitBreaker<LogStorageCircuitBreakerOptions> _logStorageCircuitBreaker;
    
    public CriticalLogsController(
        ... ,
        ICircuitBreaker<LogStorageCircuitBreakerOptions> logStorageCircuitBreaker)
    {
        _logStorageCircuitBreaker = logStorageCircuitBreaker;
    }
```


And use

```cs
[HttpPost]
public async Task<ActionResult<SavedLogResponse>> Post(SaveCriticalLogRequest request, CancellationToken token)
{
    if (await _circuitBreaker.IsOpenAsync(token))
    {
        // Fallback logic
        return StatusCode((int) HttpStatusCode.ServiceUnavailable);
    }

    try
    {
        var result = await _circuitBreaker.ExecuteAsync(async _ => 
            await _logStorage.SaveLogAsync(request.LogMessage), token);

        if (result.IsLogSaved)
            return Ok(SavedLogResponse.Successful);

        return SavedLogResponse.Failed(LogStorageFailureReason.Unknown);

    }
    catch (EventStoreConnectionException exception)
    {
        return SavedLogResponse.Failed(exception.FailureReason);
    }
}
```


## Concurrency conflict resolution
What happens if there are two requests that simultaniously update circuit breaker state ?
Well, for now chosen strategy is Last-Update-Wins, because it's easier and reasonable enough (but of course not the best option).

## Circuit Breaker Storage

Currently available storages are
- MongoDB
- MSSQL

Redis is coming soon.

## Technical debt
- Wrap unit tests into docker-compose
- Add comments in code
- Add logging in CircuitBreaker state handlers
- Consider renaming of some classes
- Add unit tests for untested components 
- Add Redis implementation

## Future improvements
- Ability to forcefully swtich Circuit Breaker to Open State
- Ability to forcefully swtich Circuit Breaker to Closed State
- Add more callbacks support during Circuit Breaker state changes
- Instead of using DurationOfBreak to swtich CircuitBreaker to Half-Open state we can instead periodically ping remote service (health-checks) to determine whether it become available or not.
