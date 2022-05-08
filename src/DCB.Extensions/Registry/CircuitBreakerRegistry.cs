namespace DCB.Extensions.Registry;

/// <summary>
/// TODO: Write unit tests
/// </summary>
public class CircuitBreakerRegistry
{
    private readonly Dictionary<string, CircuitBreakerRegistryInfo> _registryInfos = new();
    
    public void Add(CircuitBreakerRegistryInfo registryInfo)
    {
        if (registryInfo is null)
            throw new ArgumentNullException(nameof(registryInfo));

        if (_registryInfos.ContainsKey(registryInfo.Name))
            throw new CircuitBreakerRegistryException(
                $"CircuitBreaker with '{registryInfo.Name}' has been already registered");

        _registryInfos.Add(registryInfo.Name, registryInfo);
    }
}