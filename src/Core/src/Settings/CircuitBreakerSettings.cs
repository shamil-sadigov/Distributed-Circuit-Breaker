using Core.Settings.Handlers.ExceptionHandlers;
using Core.Settings.Handlers.ResultHandlers;

namespace Core.Settings;

public abstract partial class CircuitBreakerSettings : ICircuitBreakerSettings
{
    public abstract string Name { get; }
    public abstract int FailureAllowedBeforeBreaking { get; }
    public abstract TimeSpan DurationOfBreak { get; }
    
    private ExceptionHandlers ExceptionHandlers { get; } = new();
    private ResultHandlers ResultHandlers { get; } = new();
}