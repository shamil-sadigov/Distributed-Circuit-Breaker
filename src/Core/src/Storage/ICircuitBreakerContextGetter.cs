using Core.CircuitBreakers.Context;

namespace Core.Storage;

public interface ICircuitBreakerContextGetter
{
    Task<CircuitBreakerState?> GetAsync(string circuitBreakerName, CancellationToken token);
}