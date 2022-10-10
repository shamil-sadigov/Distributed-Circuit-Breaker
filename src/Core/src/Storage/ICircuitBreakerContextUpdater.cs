using Core.CircuitBreakers.Context;
using Core.Exceptions;

namespace Core.Storage;

public interface ICircuitBreakerContextUpdater
{
    /// <exception cref="CircuitBreakerSnapshotNotFoundException">
    ///     When snapshot.Name doesn't exists in storage
    /// </exception>
    /// <returns></returns>
    Task UpdateAsync(CircuitBreakerState state, CancellationToken token);
}