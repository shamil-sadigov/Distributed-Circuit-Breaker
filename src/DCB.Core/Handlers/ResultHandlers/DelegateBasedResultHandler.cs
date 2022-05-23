using DCB.Shared;

namespace DCB.Core.Handlers.ResultHandlers;

public class DelegateBasedResultHandler<TResult> : IResultHandler<TResult>
{
    private readonly Func<TResult, bool> _handleResult;

    public DelegateBasedResultHandler(Func<TResult, bool> handleResult)
    {
        handleResult.ThrowIfNull();

        _handleResult = handleResult;
    }

    public bool HandleResult(TResult result)
    {
        return _handleResult(result);
    }
}