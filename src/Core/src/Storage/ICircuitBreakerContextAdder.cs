using Core.CircuitBreakers.Context;

namespace Core.Storage;

// TODO: Add in contracts that Duplicate context Exception may occur do to duplicate names due to concurrency issues

public interface ICircuitBreakerContextAdder
{
    Task AddAsync(CircuitBreakerContextSnapshot snapshot, CancellationToken token);
}