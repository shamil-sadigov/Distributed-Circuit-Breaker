using Core.Policy.Handlers.ResultHandlers;
using Helpers;

namespace Core.Policy;

public partial class CircuitBreakerPolicy
{
    public CircuitBreakerPolicy HandleResult<TResult>(Func<TResult, bool> resultHandler)
    {
        resultHandler.ThrowIfNull();
        ResultHandlers.Handle(resultHandler);
        return this;
    }

    public CircuitBreakerPolicy HandleResult<TResult>(IResultHandler<TResult> resultHandler)
    {
        resultHandler.ThrowIfNull();
        ResultHandlers.Handle(resultHandler);
        return this;
    }
    
    internal bool ShouldHandleResult<TResult>(TResult result) 
        => ResultHandlers.CanHandle(result);
}