using DCB.Helpers;

namespace DCB.Core.Handlers.ExceptionHandlers;

/// <summary>
/// TODO: Write unit tests
/// </summary>
internal sealed class ExceptionHandlers
{
    private readonly HashSet<IExceptionHandler> _exceptionHandlers = new();
    
    internal ExceptionHandlers Handle(IExceptionHandler exceptionHandler)
    {
        exceptionHandler.ThrowIfNull();
        _exceptionHandlers.Add(exceptionHandler);

        return this;
    }

    internal ExceptionHandlers Handle<TException>(Func<TException, bool> exceptionHandler) where TException : Exception
    {
        exceptionHandler.ThrowIfNull();
        var handler = new DelegateBasedExceptionHandler(ex => ex is TException typedEx && exceptionHandler(typedEx));
        _exceptionHandlers.Add(handler);
        return this;
    }
    
    internal bool CanHandle(Exception exception) => _exceptionHandlers.Any(x => x.HandleException(exception));
}