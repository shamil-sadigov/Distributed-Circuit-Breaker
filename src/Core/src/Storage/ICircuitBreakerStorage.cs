using Core.Exceptions;

namespace Core.Storage;

public interface ICircuitBreakerStorage
{
    /// <returns>Null if not found</returns>
    Task<CircuitBreakerSnapshot?> GetAsync(string circuitBreakerName, CancellationToken token);
    
    /// <exception cref="CircuitBreakerSnapshotNotFoundException">
    ///     When snapshot.Name doesn't exists in storage
    /// </exception>
    /// <returns></returns>
    Task UpdateAsync(CircuitBreakerSnapshot snapshot, CancellationToken token);
    
    
    Task AddAsync(CircuitBreakerSnapshot snapshot, CancellationToken token);
}