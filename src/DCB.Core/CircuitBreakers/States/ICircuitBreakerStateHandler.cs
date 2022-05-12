using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers.States;

internal interface ICircuitBreakerStateHandler
{
    Task<TResult> HandleAsync<TResult>(
        CircuitBreakerOptions options,
        ICircuitBreakerStore store, 
        Func<Task<TResult>> action, 
        CircuitBreakerData data);
}