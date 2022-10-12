using Core.Policy.Handlers.ExceptionHandlers;
using Core.Policy.Handlers.ResultHandlers;

namespace Core.Policy;

/// <summary>
/// Base Policy class that should be inherited by client and configured   
/// </summary>
public abstract partial class CircuitBreakerPolicy
{
    /// <summary>
    /// Unique name of circuit breaker. Should not be greater than 256
    /// </summary>
    public abstract string Name { get; }
    
    /// <summary>
    /// Number of failures allowed before breaking circuit breaker (e.g moving circuit breaker in Open state).
    /// </summary>
    public abstract int FailureAllowed { get; set; }
    
    /// <summary>
    /// How long does circuit breaker is allowed to be in Open state
    /// </summary>
    public abstract TimeSpan DurationOfBreak { get; set; }
    
    /// <summary>
    /// Which exception should be handled by circuit breaker
    /// </summary>
    private ExceptionHandlers ExceptionHandlers { get; } = new();
    
    /// <summary>
    /// Which results should be handled by circuit breaker
    /// </summary>
    private ResultHandlers ResultHandlers { get; } = new();
}