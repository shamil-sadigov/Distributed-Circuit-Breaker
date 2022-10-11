namespace Core.Exceptions;

// TODO: Maybe use InvalidCiruicBrekaerState Exception
public class CircuitBreakerIsOpenException : CircuitBreakerException
{
    public CircuitBreakerIsOpenException(string? message) : base(message)
    {
    }
}