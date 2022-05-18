using DCB.Core.CircuitBreakers.Context;

namespace DCB.Core.Storage;

public interface ICircuitBreakerContextAdder
{
    Task AddAsync(CircuitBreakerContextSnapshot snapshot, CancellationToken token);
}