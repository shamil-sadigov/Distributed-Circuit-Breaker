using DCB.Core.CircuitBreakers;
using DCB.Core.CircuitBreakers.States;

namespace DCB.Core;

// TODO: Register in IoC
public interface ICircuitBreakerStore
    :ICircuitBreakerContextGetter, ICircuitBreakerContextSaver
{

}


public interface ICircuitBreakerContextGetter
{
    Task<CircuitBreakerContext> GetAsync(string circuitBreakerName);
}

public interface ICircuitBreakerContextSaver
{
    Task SaveAsync(CircuitBreakerContext context);
}