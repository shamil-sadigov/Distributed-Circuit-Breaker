using DCB.Shared;

namespace DCB.Core.Handlers.ExceptionHandlers;

/// <summary>
///     TODO: Write unit tests
/// </summary>
public sealed class ExceptionHandlers
{
    private readonly HashSet<IExceptionHandler> _exceptionHandlers = new();

    public ExceptionHandlers Handle(IExceptionHandler exceptionHandler)
    {
        exceptionHandler.ThrowIfNull();
        _exceptionHandlers.Add(exceptionHandler);

        return this;
    }

    public ExceptionHandlers Handle<TException>()
        where TException : Exception
    {
        var handler = new DelegateBasedExceptionHandler(ex => ex is TException);
        _exceptionHandlers.Add(handler);
        return this;
    }

    public ExceptionHandlers Handle<TException>(Func<TException, bool> exceptionHandler) where TException : Exception
    {
        exceptionHandler.ThrowIfNull();
        var handler = new DelegateBasedExceptionHandler(ex => ex is TException typedEx && exceptionHandler(typedEx));
        _exceptionHandlers.Add(handler);
        return this;
    }

    internal bool CanHandle(Exception exception)
    {
        return _exceptionHandlers.Any(x => x.HandleException(exception));
    }
}