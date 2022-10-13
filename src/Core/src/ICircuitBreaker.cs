using Core.Context;
using Core.Policy;

namespace Core;

public interface ICircuitBreaker<out TPolicy> where TPolicy : CircuitBreakerPolicy
{
    TPolicy Policy { get; }
    Task<CircuitBreakerState> GetStateAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// How many times action execution failed
    /// </summary>
    Task<int> GetFailedTimesAsync(CancellationToken cancellationToken);
    
    Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken);
    
    Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action, 
        CancellationToken cancellationToken);
}