using DCB.Core.Handlers.ExceptionHandlers;
using DCB.Helpers;

namespace DCB.Core.CircuitBreakerOptions;

public partial class CircuitBreakerOptions<TResult>
{
    public CircuitBreakerOptions<TResult> HandleException<TException>() 
        where TException : Exception
    {
        var handler = new DelegateBasedExceptionHandler(ex => ex is TException);
        HandleException(handler);
        return this;
    }
    
    public CircuitBreakerOptions<TResult> HandleException<TException>(Func<TException, bool> exceptionHandler) 
        where TException : Exception
    {
        exceptionHandler.ThrowIfNull();
        var handler = new DelegateBasedExceptionHandler(ex => ex is TException typedEx && exceptionHandler(typedEx));
        HandleException(handler);
        return this;
    }

    public CircuitBreakerOptions<TResult> HandleException(IExceptionHandler exceptionHandler)
    {
        exceptionHandler.ThrowIfNull();
        ExceptionHandlers ??= new ExceptionHandlers();
        ExceptionHandlers.Add(exceptionHandler);
        return this;
    }
}