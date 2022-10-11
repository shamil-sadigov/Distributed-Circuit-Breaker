using Core.Exceptions;

namespace Extensions.Microsoft.DependencyInjection;

public class CircuitBreakerRegistrationException : CircuitBreakerException
{
    public CircuitBreakerRegistrationException(string? message)
        : base(message)
    {
    }
}