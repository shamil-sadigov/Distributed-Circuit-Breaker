[WIP]

# Distributed-Circuit-Breaker

### Description
This repo is a proof of concept of distributed circuit breaker. 

Distributed Circuit Breaker can be helpful if you need to share a circuit breaker among multiple services (or multiple instances of the same service), which means that circuit breaker's state is kept in external storage (Redis, Mongo and so on...) as opposed to service instance memory. Why do we beed it ? Well, go to `Problem` section.


### Context
Goto solution for appliying circuit breakers is lovely [Polly](https://github.com/App-vNext/Polly) library. It allows you to nicely create/use/reuse circuit breaker that can be shared across the application code. 

Circuit breaker can be thought as a resilient wrapper of the external system state which allows you to prevent any overwheming external system with redundant request and fail fast or fallback when the system is not healthy. But what if we want to share the same circuit breaker with other services in order for them to be also aware of that the external system unhealthiness ? 


### Problem

In below image, we have two services.
- Trace-log-saver => Saves all trace-level logs in centralized log storage  
- Critical-log-saver => Do the same thing but only for critical-level logs.

Both of these services use Circuit Breaker when dealing with Log Storage.
And let say that Log Storage went down.
Trace-log-saver noticed it after several failed attempts to send logs to Log Storage, and switched CircuitBreaker to Open state.
But Critical-Log-saver is not aware yet about unhealthy state of Log Storage, which means that Critical-Log-saver should also make several failed attempts in order to realize that Log Storage is unhealthy and turn CircuitBreaker to Open state.

![stateless-circuit-breaker](https://github.com/shamil-sadigov/Distributed-Circuit-Breaker/blob/main/docs/images/Small%20ones/problem-of-in-memory-circuit-breaker.jpg)



### Solution
TODO

### Concurrency conflicts
TODO


### Technical debt
TODO
- Redis instead of Mongo

### Future improvements
TODO
