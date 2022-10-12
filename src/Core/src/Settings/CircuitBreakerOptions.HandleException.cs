using Core.Settings.Handlers.ExceptionHandlers;
using Helpers;

namespace Core.Settings;

public partial class CircuitBreakerSettings
{
    public CircuitBreakerSettings HandleException<TException>()
        where TException : Exception
    {
        var handler = new DelegateBasedExceptionHandler(ex => ex is TException);
        ExceptionHandlers.Handle(handler);
        return this;
    }

    public CircuitBreakerSettings HandleException<TException>(Func<TException, bool> exceptionHandler)
        where TException : Exception
    {
        exceptionHandler.ThrowIfNull();
        ExceptionHandlers.Handle(exceptionHandler);
        return this;
    }
    
    public CircuitBreakerSettings HandleException(IExceptionHandler exceptionHandler)
    {
        exceptionHandler.ThrowIfNull();
        ExceptionHandlers.Handle(exceptionHandler);
        return this;
    }
    
    internal bool ShouldHandleException(Exception exception) 
        => ExceptionHandlers.CanHandle(exception);
}