using DCB.Core.Handlers.ExceptionHandlers;
using DCB.Core.Handlers.ResultHandlers;
using DCB.Helpers;

namespace DCB.Core.CircuitBreakerOptions;

public partial class CircuitBreakerOptions
{
    public CircuitBreakerOptions HandleResult<TResult>(Func<TResult, bool> resultHandler) 
    {
        resultHandler.ThrowIfNull();
        var handler = new DelegateBasedResultHandler<TResult>(resultHandler);
        HandleResult(handler);
        return this;
    }

    public CircuitBreakerOptions HandleResult<TResult>(IResultHandler<TResult> resultHandler)
    {
        resultHandler.ThrowIfNull();
        ResultHandlers ??= new ResultHandlers();
        ResultHandlers.Add(resultHandler);
        return this;
    }
}