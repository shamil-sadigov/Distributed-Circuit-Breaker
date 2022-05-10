using DCB.Helpers;

namespace DCB.Core.Handlers.ResultHandlers;

/// <summary>
/// TODO: Write tests
/// </summary>
/// <typeparam name="TResult"></typeparam>
internal class ResultHandlers<TResult>
{
    protected readonly HashSet<IResultHandler<TResult>> _resultHandlers = new();
    
    internal void Add(IResultHandler<TResult> resultHandler)
    {
        resultHandler.ThrowIfNull();
        _resultHandlers.Add(resultHandler);
    }
    
    internal bool CanBeHandled(TResult result) => _resultHandlers.Any(x => x.HandleResult(result));
}