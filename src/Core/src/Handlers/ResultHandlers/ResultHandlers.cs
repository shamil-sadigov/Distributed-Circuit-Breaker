using Helpers;

namespace Core.Handlers.ResultHandlers;

/// <summary>
///     TODO: Write tests
/// </summary>
public class ResultHandlers
{
    private readonly List<object> _resultHandlers = new();

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

    // TODO: Rename it. Maybe HandleAvailable ?
    internal bool CanHandle<TResult>(TResult result)
    {
        return _resultHandlers
            .OfType<IResultHandler<TResult>>()
            .Any(x => x.HandleResult(result));
    }
}