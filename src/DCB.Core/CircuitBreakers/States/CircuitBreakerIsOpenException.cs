namespace DCB.Core.CircuitBreakers.States;

public class CircuitBreakerIsOpenException:CircuitBreakerException
{
    public CircuitBreakerIsOpenException(string? message) : base(message)
    {
    }
}