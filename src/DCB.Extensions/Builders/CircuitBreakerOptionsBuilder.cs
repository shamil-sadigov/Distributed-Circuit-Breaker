using DCB.Core.CircuitBreakerOption;
using DCB.Extensions.Registry;

namespace DCB.Extensions.Builders;

public class CircuitBreakerOptionsBuilder
{
    public CircuitBreakerRegistry CircuitBreakerRegistry { get; }
    
    public CircuitBreakerOptionsBuilder(CircuitBreakerRegistry circuitBreakerRegistry)
    {
        CircuitBreakerRegistry = circuitBreakerRegistry;
    }
    
    public CircuitBreakerOptionsBuilder AddCircuitBreaker<TOptions>(TOptions options) 
        where TOptions : CircuitBreakerOptionsBase
    {
        CircuitBreakerRegistry.Add(options);
        return this;
    }
}