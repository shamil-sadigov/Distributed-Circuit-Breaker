using DCB.Core;
using DCB.Core.CircuitBreakerOption;
using DCB.Extensions.Registry;

namespace DCB.Extensions;

public class CircuitBreakerContextBuilder
{
    private Type? _circuitBreakerStorageType;
    private CircuitBreakerRegistry CircuitBreakerRegistry { get; } = new();
    
    public CircuitBreakerContextBuilder AddCircuitBreaker<TOptions>(TOptions options) 
        where TOptions : CircuitBreakerOptionsBase
    {
        CircuitBreakerRegistry.Add(options);
        return this;
    }

    public CircuitBreakerContextBuilder UseStorage<TStorage>()
        where TStorage: ICircuitBreakerStore
    {
        _circuitBreakerStorageType = typeof(TStorage);
        return this;
    }

    public CircuitBreakerContext Build()
    {
        if (_circuitBreakerStorageType is null)
        {
            throw new InvalidOperationException(
                $"Storage is not specified. Ensure you specified it via {nameof(UseStorage)}()");
        }

        var options = CircuitBreakerContext
            .Create(CircuitBreakerRegistry.GetAll(), _circuitBreakerStorageType);
        
        return options;
    }
}