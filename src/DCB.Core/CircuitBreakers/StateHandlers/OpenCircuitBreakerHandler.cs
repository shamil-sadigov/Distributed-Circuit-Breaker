using DCB.Core.CircuitBreakerOption;
using DCB.Core.CircuitBreakers.Context;
using DCB.Core.Exceptions;

namespace DCB.Core.CircuitBreakers.StateHandlers;

internal sealed class OpenCircuitBreakerHandler : ICircuitBreakerStateHandler
{
    public Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CircuitBreakerOptions options, CancellationToken token)
    {
        EnsureCircuitBreakerIsOpen(circuitBreaker);

        throw new CircuitBreakerIsOpenException($"Circuit breaker with name '{circuitBreaker.Name}' cannot be used" +
                                                "while it's in open state");
    }

    public bool CanHandle(CircuitBreakerContext context)
    {
        return context.State == CircuitBreakerStateEnum.Open;
    }

    private void EnsureCircuitBreakerIsOpen(CircuitBreakerContext context)
    {
        if (!CanHandle(context))
            throw new InvalidCircuitBreakerStateException(context.Name, context.State, CircuitBreakerStateEnum.Open);
    }
}