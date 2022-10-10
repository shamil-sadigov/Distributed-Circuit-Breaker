using Core.CircuitBreakers.Context;

namespace Core.Storage;

public interface ICircuitBreakerContextGetter
{
    Task<CircuitBreakerContextSnapshot?> GetAsync(string circuitBreakerName, CancellationToken token);
}