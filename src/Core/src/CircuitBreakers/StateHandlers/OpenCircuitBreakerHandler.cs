using Core.CircuitBreakerOption;
using Core.CircuitBreakers.Context;
using Core.Exceptions;

namespace Core.CircuitBreakers.StateHandlers;

// TODO: Ensure that handler is really registed in DI
internal sealed class OpenCircuitBreakerHandler : ICircuitBreakerStateHandler
{
    public Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CircuitBreakerSettings settings, 
        CancellationToken token)
    {
        throw new CircuitBreakerIsOpenException($"Circuit breaker with name '{circuitBreaker.Name}' cannot be used" +
                                                " while it's in open state");
    }

    public bool CanHandle(CircuitBreakerContext context)
    {
        return context.State is CircuitBreakerState.Open;
    }
}