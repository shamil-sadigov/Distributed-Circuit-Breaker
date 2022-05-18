using DCB.Core.CircuitBreakers.Context;

namespace DCB.Core.Storage;

public interface ICircuitBreakerContextGetter
{
    Task<CircuitBreakerContextSnapshot?> GetAsync(string circuitBreakerName, CancellationToken token);
}