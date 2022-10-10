namespace Core.Handlers.ExceptionHandlers;

public class DelegateBasedExceptionHandler : IExceptionHandler
{
    private readonly Func<Exception, bool> _func;

    public DelegateBasedExceptionHandler(Func<Exception, bool> func)
    {
        _func = func;
    }


    public bool HandleException<TException>(TException ex) where TException : Exception
    {
        return _func(ex);
    }
}