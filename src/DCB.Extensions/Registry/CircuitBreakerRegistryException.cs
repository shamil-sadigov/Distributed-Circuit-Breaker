using DCB.Core;

namespace DCB.Extensions.Registry;

public class CircuitBreakerRegistryException : CircuitBreakerException
{
    public CircuitBreakerRegistryException(string? message)
        : base(message)
    {
    }
}