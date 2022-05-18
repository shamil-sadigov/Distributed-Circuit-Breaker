using DCB.Helpers;

namespace DCB.Core.Handlers.ResultHandlers;

/// <summary>
///     TODO: Write tests
/// </summary>
/// <typeparam name="TResult"></typeparam>
public class ResultHandlers
{
    protected readonly List<object> _resultHandlers = new();

    public ResultHandlers Handle<TResult>(Func<TResult, bool> resultHandler)
    {
        resultHandler.ThrowIfNull();
        var handler = new DelegateBasedResultHandler<TResult>(resultHandler);
        _resultHandlers.Add(handler);
        return this;
    }

    public ResultHandlers Handle<TResult>(IResultHandler<TResult> resultHandler)
    {
        resultHandler.ThrowIfNull();
        _resultHandlers.Add(resultHandler);
        return this;
    }

    public bool CanHandle<TResult>(TResult result)
    {
        return _resultHandlers
            .OfType<IResultHandler<TResult>>()
            .Any(x => x.HandleResult(result));
    }
}