using DCB.Core.CircuitBreakers.CircuitBreakerContext;
using DCB.Core.Exceptions;

namespace DCB.Core.Storage;

public interface ICircuitBreakerContextUpdater
{
    /// <exception cref="CircuitBreakerSnapshotNotFoundException">
    /// When snapshot.Name doesn't exists in storage
    /// </exception>
    /// <returns></returns>
    Task UpdateAsync(CircuitBreakerContextSnapshot snapshot, CancellationToken token);
}