using Core.Handlers.ResultHandlers;
using Shared;

namespace Core.CircuitBreakerOption;

public partial class CircuitBreakerOptions
{
    public CircuitBreakerOptions HandleResult<TResult>(Func<TResult, bool> resultHandler)
    {
        resultHandler.ThrowIfNull();
        ResultHandlers.Handle(resultHandler);
        return this;
    }

    public CircuitBreakerOptions HandleResult<TResult>(IResultHandler<TResult> resultHandler)
    {
        resultHandler.ThrowIfNull();
        ResultHandlers.Handle(resultHandler);
        return this;
    }
}