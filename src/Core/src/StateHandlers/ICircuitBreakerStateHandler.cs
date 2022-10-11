using Core.Context;

namespace Core.StateHandlers;

public interface ICircuitBreakerStateHandler
{
    Task<TResult> HandleAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CancellationToken token);

    public bool CanHandle(CircuitBreakerState state);
}