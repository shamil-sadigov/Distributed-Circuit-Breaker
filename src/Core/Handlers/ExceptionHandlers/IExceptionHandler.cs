namespace Core.Handlers.ExceptionHandlers;

public interface IExceptionHandler
{
    bool HandleException<TException>(TException ex) where TException : Exception;
}