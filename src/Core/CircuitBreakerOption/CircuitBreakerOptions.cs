using Core.Handlers.ExceptionHandlers;
using Core.Handlers.ResultHandlers;

namespace Core.CircuitBreakerOption;

public abstract partial class CircuitBreakerOptions : CircuitBreakerOptionsBase
{
    internal ExceptionHandlers ExceptionHandlers { get; } = new();
    internal ResultHandlers ResultHandlers { get; } = new();
}