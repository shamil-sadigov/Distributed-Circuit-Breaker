using Core.CircuitBreakers;
using Core.Exceptions;

namespace Core.Storage;

public interface ICircuitBreakerContextUpdater
{
    /// <exception cref="CircuitBreakerSnapshotNotFoundException">
    ///     When snapshot.Name doesn't exists in storage
    /// </exception>
    /// <returns></returns>
    Task UpdateAsync(CircuitBreakerSnapshot snapshot, CancellationToken token);
}