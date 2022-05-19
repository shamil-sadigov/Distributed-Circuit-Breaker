using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers;

public interface ICircuitBreaker<TOptions> where TOptions : CircuitBreakerOptionsBase
{
    Task<CircuitBreakerState> GetStateAsync();
    
    Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken);
}