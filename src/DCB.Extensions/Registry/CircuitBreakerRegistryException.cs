namespace DCB.Extensions;

public class CircuitBreakerRegistryException:Exception
{
    public CircuitBreakerRegistryException(string? message) 
        : base(message)
    {
    }
}