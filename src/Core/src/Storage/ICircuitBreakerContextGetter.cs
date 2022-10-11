namespace Core.Storage;

public interface ICircuitBreakerContextGetter
{
    Task<CircuitBreakerSnapshot?> GetAsync(string circuitBreakerName, CancellationToken token);
}