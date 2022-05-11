using DCB.Core;
using DCB.Core.CircuitBreakerOptions;
using DCB.Helpers;

namespace DCB.Extensions;

public class CircuitBreakerContext
{
    public static CircuitBreakerContext Create(
        IReadOnlyCollection<CircuitBreakerOptionsBase> options,
        Type storageType)
    {
        storageType.ThrowIfNull();
        
        if (!storageType.IsAssignableFrom(typeof(ICircuitBreakerStore)))
            throw new ArgumentException($"Should be of type '{nameof(ICircuitBreakerStore)}'", nameof(storageType));

        if (options.Count == 0)
            throw new ArgumentException("Should contain at least one option", nameof(options));

        return new CircuitBreakerContext(options, storageType);
    }
    
    private CircuitBreakerContext(
        IReadOnlyCollection<CircuitBreakerOptionsBase> circuitBreakerOptions,
        Type storageType)
    {
        CircuitBreakerOptions = circuitBreakerOptions;
        StorageType = storageType;
    }
    
    public IReadOnlyCollection<CircuitBreakerOptionsBase> CircuitBreakerOptions { get;  }
    public OptionsExtensions Extensions { get; } = new();
    
    
    /// <summary>
    /// Implementation type of <see cref="ICircuitBreakerStore"/>
    /// </summary>
    public Type StorageType { get; }
}