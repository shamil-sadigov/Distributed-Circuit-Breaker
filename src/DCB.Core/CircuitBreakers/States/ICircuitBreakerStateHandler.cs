using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers.States;

public interface ICircuitBreakerStateHandler
{
    Task<TResult> HandleAsync<TResult>(
        CircuitBreakerOptions options,
        Func<Task<TResult>> action, 
        CircuitBreakerContext context);

    public bool CanHandle(CircuitBreakerContext context);
}