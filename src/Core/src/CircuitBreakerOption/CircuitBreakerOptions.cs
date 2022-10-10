using Core.Handlers.ExceptionHandlers;
using Core.Handlers.ResultHandlers;

namespace Core.CircuitBreakerOption;

public abstract partial class CircuitBreakerOptions : CircuitBreakerOptionsBase
{
    private ExceptionHandlers ExceptionHandlers { get; } = new();
    private ResultHandlers ResultHandlers { get; } = new();

    internal bool CanHandleException(Exception exception) 
        => ExceptionHandlers.CanHandle(exception);
    
    internal bool CanHandleResult<TResult>(TResult result) 
        => ResultHandlers.CanHandle(result);

}