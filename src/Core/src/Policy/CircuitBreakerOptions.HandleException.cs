using Core.Policy.Handlers.ExceptionHandlers;
using Shared;

namespace Core.Policy;

public partial class CircuitBreakerPolicy
{
    public CircuitBreakerPolicy HandleException<TException>()
        where TException : Exception
    {
        var handler = new DelegateBasedExceptionHandler(ex => ex is TException);
        ExceptionHandlers.Handle(handler);
        return this;
    }

    public CircuitBreakerPolicy HandleException<TException>(Func<TException, bool> exceptionHandler)
        where TException : Exception
    {
        exceptionHandler.ThrowIfNull();
        ExceptionHandlers.Handle(exceptionHandler);
        return this;
    }
    
    public CircuitBreakerPolicy HandleException(IExceptionHandler exceptionHandler)
    {
        exceptionHandler.ThrowIfNull();
        ExceptionHandlers.Handle(exceptionHandler);
        return this;
    }
    
    internal bool ShouldHandleException(Exception exception) 
        => ExceptionHandlers.CanHandle(exception);
}