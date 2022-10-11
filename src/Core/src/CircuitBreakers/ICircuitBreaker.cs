using Core.CircuitBreakerOption;
using Core.CircuitBreakers.Context;

namespace Core.CircuitBreakers;

public interface ICircuitBreaker<TOptions> where TOptions : ICircuitBreakerSettings
{
    Task<CircuitBreakerState> GetStateAsync(CancellationToken cancellationToken);
    
    Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken);
    
    Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action, 
        CancellationToken cancellationToken);
}