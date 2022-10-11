using Core.Settings.Handlers.ExceptionHandlers;
using Core.Settings.Handlers.ResultHandlers;

namespace Core.Settings;

public abstract partial class CircuitBreakerSettings
{
    /// <summary>
    /// Unique name of circuit breaker
    /// </summary>
    // TODO: Ensure no longer than 256 char
    public abstract string Name { get; }
    
    /// <summary>
    /// Number of failures allowed before breaking circuit breaker (e.g moving circuit breaker in Open state).
    /// </summary>
    public abstract int FailureAllowed { get; set; }
    
    /// <summary>
    /// How long does circuit breaker is allowed to be in Open state
    /// </summary>
    public abstract TimeSpan DurationOfBreak { get; set; }
    
    private ExceptionHandlers ExceptionHandlers { get; } = new();
    private ResultHandlers ResultHandlers { get; } = new();
}