using DCB.Core.CircuitBreakers.States;

namespace DCB.Core;

// TODO: Register in IoC
public interface ICircuitBreakerStore
    :ICircuitBreakerContextGetter, ICircuitBreakerContextSaver
{

}


public interface ICircuitBreakerContextGetter
{
    Task<CircuitBreakerContextSnapshot?> GetAsync(string circuitBreakerName);
}

public interface ICircuitBreakerContextSaver
{
    // snapshot can be new or existing one
    Task SaveAsync(CircuitBreakerContextSnapshot snapshot);
}