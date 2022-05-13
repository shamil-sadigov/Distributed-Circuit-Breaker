using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers.States;

internal sealed class OpenCircuitBreakerHandler:ICircuitBreakerStateHandler
{
    public Task<TResult> HandleAsync<TResult>(
        CircuitBreakerOptions options,
        Func<Task<TResult>> action, 
        CircuitBreakerContext circuitBreaker)
    {
        EnsureCircuitBreakerIsOpen(circuitBreaker);
        
        throw new CircuitBreakerIsOpenException($"Circuit breaker with name '{circuitBreaker.Name}' cannot be used" +
                                                $"while it's in open state");
    }

    public bool CanHandle(CircuitBreakerContext context) 
        => context.State == CircuitBreakerStateEnum.Open;

    private void EnsureCircuitBreakerIsOpen(CircuitBreakerContext context)
    {
        if (!CanHandle(context))
            throw new InvalidCircuitBreakerStateException(context.Name, context.State, CircuitBreakerStateEnum.Open);
    }
}