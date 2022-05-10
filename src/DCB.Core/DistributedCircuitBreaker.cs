namespace DCB.Core;

// Make ir partial
public class DistributedCircuitBreaker<TResult>
{
    private readonly ICircuitBreakerStorage _circuitBreakerStorage;
    public CircuitBreakerOptions.CircuitBreakerOptions<TResult> Options;

    protected DistributedCircuitBreaker(
        ICircuitBreakerStorage circuitBreakerStorage, CircuitBreakerOptions.CircuitBreakerOptions<TResult> options)
    {
        _circuitBreakerStorage = circuitBreakerStorage;
        Options = options;
    }

    protected Task<TResult> ExecuteAsync(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}