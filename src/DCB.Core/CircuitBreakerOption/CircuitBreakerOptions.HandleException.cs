using DCB.Core.Handlers.ExceptionHandlers;
using DCB.Shared;

namespace DCB.Core.CircuitBreakerOption;

public partial class CircuitBreakerOptions
{
    public CircuitBreakerOptions HandleException<TException>()
        where TException : Exception
    {
        var handler = new DelegateBasedExceptionHandler(ex => ex is TException);
        ExceptionHandlers.Handle(handler);
        return this;
    }

    public CircuitBreakerOptions HandleException<TException>(Func<TException, bool> exceptionHandler)
        where TException : Exception
    {
        exceptionHandler.ThrowIfNull();
        ExceptionHandlers.Handle(exceptionHandler);
        return this;
    }

    public CircuitBreakerOptions HandleException(IExceptionHandler exceptionHandler)
    {
        exceptionHandler.ThrowIfNull();
        ExceptionHandlers.Handle(exceptionHandler);
        return this;
    }
}