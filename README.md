[WIP]

# Distributed-Circuit-Breaker

### Description
This repo is a proof of concept of distributed circuit breaker. 

Distributed Circuit Breaker can be helpful if you need to share a circuit breaker among multiple services (or multiple instances of the same service), which means that circuit breaker's state is kept in external storage (Redis, Mongo and so on...) as opposed to service instance memory. Why do we beed it ? Well, go to `Problem` section.


### Problem
Goto solution for appliying circuit breakers is lovely [Polly](https://github.com/App-vNext/Polly) library. It allows you to nicely create & use circuit breaker that can be shared across the application code. Circuit breaker is actually king of special wrapper of the state of external system which allows you to prevent any redundant request and void overwheming external system when the system is not healthy. But what to do if we would like to share the circuit breaker with other services in order for them to also know that the external system is unhealthy ?   



### Solution
TODO

### Concurrency conflicts
TODO


### Technical debt
TODO
- Redis instead of Mongo

### Future improvements
TODO
