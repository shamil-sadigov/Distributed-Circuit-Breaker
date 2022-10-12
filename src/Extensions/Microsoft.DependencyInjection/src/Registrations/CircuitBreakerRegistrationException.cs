using Core.Exceptions;

namespace Extensions.Microsoft.DependencyInjection.Registrations;

public class CircuitBreakerRegistrationException : CircuitBreakerException
{
    public CircuitBreakerRegistrationException(string? message)
        : base(message)
    {
    }
}