using Core.Handlers.ExceptionHandlers;
using Core.Handlers.ResultHandlers;

namespace Core.CircuitBreakerOption;

public abstract partial class CircuitBreakerSettings : ICircuitBreakerSettings
{
    public abstract string Name { get; }
    public abstract int FailureAllowedBeforeBreaking { get; }
    public abstract TimeSpan DurationOfBreak { get; }
    
    private ExceptionHandlers ExceptionHandlers { get; } = new();
    private ResultHandlers ResultHandlers { get; } = new();
}