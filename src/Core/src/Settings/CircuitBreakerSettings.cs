using Core.Settings.Handlers.ExceptionHandlers;
using Core.Settings.Handlers.ResultHandlers;

namespace Core.Settings;

public abstract partial class CircuitBreakerSettings
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
    /// Which exception should be handled
    /// </summary>
    private ExceptionHandlers ExceptionHandlers { get; } = new();
    
    /// <summary>
    /// Which results should be handled
    /// </summary>
    private ResultHandlers ResultHandlers { get; } = new();
}