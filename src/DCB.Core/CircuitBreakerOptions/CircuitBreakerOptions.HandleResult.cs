using DCB.Core.Handlers.ExceptionHandlers;
using DCB.Core.Handlers.ResultHandlers;
using DCB.Helpers;

namespace DCB.Core.CircuitBreakerOptions;

public partial class CircuitBreakerOptions<TResult>
{
    public CircuitBreakerOptions<TResult> HandleResult(Func<TResult, bool> resultHandler) 
    {
        resultHandler.ThrowIfNull();
        var handler = new DelegateBasedResultHandler<TResult>(resultHandler);
        HandleResult(handler);
        return this;
    }

    public CircuitBreakerOptions<TResult> HandleResult(IResultHandler<TResult> resultHandler)
    {
        resultHandler.ThrowIfNull();
        ResultHandlers ??= new ResultHandlers<TResult>();
        ResultHandlers.Add(resultHandler);
        return this;
    }
}