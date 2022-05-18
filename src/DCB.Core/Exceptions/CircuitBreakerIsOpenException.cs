namespace DCB.Core.Exceptions;

public class CircuitBreakerIsOpenException:CircuitBreakerException
{
    public CircuitBreakerIsOpenException(string? message) : base(message)
    {
    }
}