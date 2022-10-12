namespace Core.Storage;

public interface ICircuitBreakerStorage
{
    /// <returns>Null if not found</returns>
    Task<CircuitBreakerSnapshot?> GetAsync(string circuitBreakerName, CancellationToken token);
    
    Task SaveAsync(CircuitBreakerSnapshot snapshot, CancellationToken token);
}