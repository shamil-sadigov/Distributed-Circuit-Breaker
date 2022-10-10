using Core;

namespace Extensions.Registry;

public class CircuitBreakerRegistryException : CircuitBreakerException
{
    public CircuitBreakerRegistryException(string? message)
        : base(message)
    {
    }
}