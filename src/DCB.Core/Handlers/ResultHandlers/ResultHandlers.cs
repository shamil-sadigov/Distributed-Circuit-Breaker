using DCB.Helpers;

namespace DCB.Core.Handlers.ResultHandlers;

/// <summary>
/// TODO: Write tests
/// </summary>
/// <typeparam name="TResult"></typeparam>
internal class ResultHandlers
{
    protected readonly List<object> _resultHandlers = new();
    
    internal void Add<TResult>(IResultHandler<TResult> resultHandler)
    {
        resultHandler.ThrowIfNull();
        _resultHandlers.Add(resultHandler);
    }
    
    internal bool CanBeHandled<TResult>(TResult result)
    {
        return _resultHandlers
            .OfType<IResultHandler<TResult>>()
            .Any(x => x.HandleResult(result));
    }
}