using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers.States;

public interface ICircuitBreakerStateHandler
{
    Task<TResult> HandleAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CircuitBreakerOptions options,
        CancellationToken token);

    public bool CanHandle(CircuitBreakerContext context);
}