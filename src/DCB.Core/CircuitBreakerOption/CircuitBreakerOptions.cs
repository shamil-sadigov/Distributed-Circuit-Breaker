using DCB.Core.Handlers.ExceptionHandlers;
using DCB.Core.Handlers.ResultHandlers;

namespace DCB.Core.CircuitBreakerOption;

public abstract partial class CircuitBreakerOptions:CircuitBreakerOptionsBase
{
    internal ExceptionHandlers? ExceptionHandlers { get; set; }
    internal ResultHandlers? ResultHandlers { get; set; }
}