using Core.Context;
using Core.Settings;

namespace Core;

public interface ICircuitBreaker<TOptions> where TOptions : CircuitBreakerSettings
{
    Task<CircuitBreakerState> GetStateAsync(CancellationToken cancellationToken);
    
    Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken);
    
    Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action, 
        CancellationToken cancellationToken);
}