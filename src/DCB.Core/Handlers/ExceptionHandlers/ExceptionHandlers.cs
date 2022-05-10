using DCB.Helpers;

namespace DCB.Core.Handlers.ExceptionHandlers;

/// <summary>
/// TODO: Write unit tests
/// </summary>
internal sealed class ExceptionHandlers
{
    private readonly HashSet<IExceptionHandler> _exceptionHandlers = new();
    
    internal void Add(IExceptionHandler exceptionHandler)
    {
        exceptionHandler.ThrowIfNull();
        _exceptionHandlers.Add(exceptionHandler);
    }

    internal bool CanBeHandled(Exception exception) => _exceptionHandlers.Any(x => x.HandleException(exception));
}