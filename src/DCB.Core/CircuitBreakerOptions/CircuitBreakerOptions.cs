using DCB.Core.Handlers.ExceptionHandlers;
using DCB.Core.Handlers.ResultHandlers;
using DCB.Helpers;

namespace DCB.Core.CircuitBreakerOptions;

public abstract partial class CircuitBreakerOptions:CircuitBreakerOptionsBase
{
    internal ExceptionHandlers? ExceptionHandlers { get; set; }
    internal ResultHandlers? ResultHandlers { get; set; }
}