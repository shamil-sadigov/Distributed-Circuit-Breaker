namespace DCB.Extensions.Registry;

public class CircuitBreakerRegistryException:Exception
{
    public CircuitBreakerRegistryException(string? message) 
        : base(message)
    {
    }
}