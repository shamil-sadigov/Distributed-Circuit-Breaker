using Core.CircuitBreakerOption;

namespace Core.CircuitBreakers;

public interface ICircuitBreaker<TOptions> where TOptions : CircuitBreakerOptionsBase
{
    Task<CircuitBreakerState> GetStateAsync(CancellationToken cancellationToken);

    Task<bool> IsClosedAsync(CancellationToken cancellationToken);

    Task<bool> IsOpenAsync(CancellationToken cancellationToken);
    
    Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken);
    
    Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action, 
        CancellationToken cancellationToken);
}