using Core.Settings.Handlers.ResultHandlers;
using Helpers;

namespace Core.Settings;

public partial class CircuitBreakerSettings
{
    public CircuitBreakerSettings HandleResult<TResult>(Func<TResult, bool> resultHandler)
    {
        resultHandler.ThrowIfNull();
        ResultHandlers.Handle(resultHandler);
        return this;
    }

    public CircuitBreakerSettings HandleResult<TResult>(IResultHandler<TResult> resultHandler)
    {
        resultHandler.ThrowIfNull();
        ResultHandlers.Handle(resultHandler);
        return this;
    }
    
    internal bool ShouldHandleResult<TResult>(TResult result) 
        => ResultHandlers.CanHandle(result);
}