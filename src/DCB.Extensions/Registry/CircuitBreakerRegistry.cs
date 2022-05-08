namespace DCB.Extensions;

/// <summary>
/// TODO: Write unit tests
/// </summary>
public class CircuitBreakerRegistry
{
    private readonly Dictionary<string, CircuitBreakerRegistryInfo> _circuitBreakers = new();
    
    public void Add(CircuitBreakerRegistryInfo registryInfo)
    {
        if (registryInfo is null)
            throw new ArgumentNullException(nameof(registryInfo));

        if (_circuitBreakers.ContainsKey(registryInfo.Name))
            throw new CircuitBreakerRegistryException(
                $"CircuitBreaker with '{registryInfo.Name}' has been already registered");

        _circuitBreakers.Add(registryInfo.Name, registryInfo);
    }
}