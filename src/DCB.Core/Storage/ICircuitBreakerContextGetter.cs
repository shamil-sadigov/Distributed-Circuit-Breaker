using DCB.Core.CircuitBreakers.CircuitBreakerContext;

namespace DCB.Core.Storage;

public interface ICircuitBreakerContextGetter
{
    Task<CircuitBreakerContextSnapshot?> GetAsync(string circuitBreakerName, CancellationToken token);
}