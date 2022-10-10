using Core;

namespace Registration;

public class CircuitBreakerRegistrationException : CircuitBreakerException
{
    public CircuitBreakerRegistrationException(string? message)
        : base(message)
    {
    }
}