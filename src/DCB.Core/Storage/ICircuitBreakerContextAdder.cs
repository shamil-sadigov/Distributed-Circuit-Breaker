using DCB.Core.CircuitBreakers.CircuitBreakerContext;

namespace DCB.Core.Storage;

public interface ICircuitBreakerContextAdder
{
    Task AddAsync(CircuitBreakerContextSnapshot snapshot, CancellationToken token);
}