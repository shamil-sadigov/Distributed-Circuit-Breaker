using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers.States;

internal interface ICircuitBreakerState
{
    Task<TResult> ExecuteAsync<TResult>(
        CircuitBreakerOptions options, 
        ICircuitBreakerStore store);
}