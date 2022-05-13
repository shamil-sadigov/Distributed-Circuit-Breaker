using DCB.Core;
using DCB.Core.CircuitBreakers.States;

namespace DCB.Extensions.Registry;

public class CircuitBreakerRegistryException:CircuitBreakerException
{
    public CircuitBreakerRegistryException(string? message) 
        : base(message)
    {
    }
}