using Core.Context;
using Core.Exceptions;
using Core.Helpers;

namespace Core.StateHandlers;

// TODO: Ensure that handler is really registed in DI
internal sealed class OpenCircuitBreakerHandler : ICircuitBreakerStateHandler
{
    public Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CancellationToken token)
    {
        circuitBreaker.EnsureStateIs(CircuitBreakerState.Open);
        
        throw new CircuitBreakerIsOpenException($"Circuit breaker with name '{circuitBreaker.Name}' cannot be used" +
                                                " while it's in open state");
    }
    
    public bool CanHandle(CircuitBreakerState state) => state is CircuitBreakerState.Open;
}