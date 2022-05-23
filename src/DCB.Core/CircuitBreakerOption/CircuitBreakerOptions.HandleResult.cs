using DCB.Core.Handlers.ResultHandlers;
using DCB.Shared;

namespace DCB.Core.CircuitBreakerOption;

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