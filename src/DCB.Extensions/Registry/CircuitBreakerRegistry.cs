namespace DCB.Extensions.Registry;

/// <summary>
/// TODO: Write unit tests
/// </summary>
public sealed class CircuitBreakerRegistry
{
    private readonly Dictionary<string, CircuitBreakerRegistryInfo> _registryInfos = new();
    
    public void Add(CircuitBreakerRegistryInfo registryInfo)
    {
        if (registryInfo is null)
            throw new ArgumentNullException(nameof(registryInfo));

        if (_registryInfos.ContainsKey(registryInfo.Name))
            throw new CircuitBreakerRegistryException(
                $"CircuitBreaker with name '{registryInfo.Name}' has already been registered");

        _registryInfos.Add(registryInfo.Name, registryInfo);
    }
}