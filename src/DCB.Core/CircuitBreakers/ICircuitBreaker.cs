using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers;

public interface ICircuitBreaker<TOptions> where TOptions : CircuitBreakerOptionsBase
{
    Task<CircuitBreakerState> GetStateAsync();

    Task<bool> IsClosedAsync();

    Task<bool> IsOpenAsync();
    
    Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken);
    
    Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action, 
        CancellationToken cancellationToken);
}