using DCB.Core.Handlers.ExceptionHandlers;
using DCB.Core.Handlers.ResultHandlers;
using DCB.Helpers;

namespace DCB.Core.CircuitBreakerOptions;

public partial class CircuitBreakerOptions<TResult>
{
    public string Name { get; }
    
    internal ExceptionHandlers? ExceptionHandlers { get; set; }
    internal ResultHandlers<TResult>? ResultHandlers { get; set; }
    
    protected CircuitBreakerOptions(string name)
    {
        name.ThrowIfNull();
        Name = name;
    }
}