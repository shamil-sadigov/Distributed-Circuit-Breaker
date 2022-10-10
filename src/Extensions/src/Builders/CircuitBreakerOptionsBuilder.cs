using Core.CircuitBreakerOption;
using Extensions.Registry;

namespace Extensions.Builders;

public class CircuitBreakerOptionsBuilder
{
    public CircuitBreakerOptionsBuilder(CircuitBreakerRegistry circuitBreakerRegistry)
    {
        CircuitBreakerRegistry = circuitBreakerRegistry;
    }

    internal CircuitBreakerRegistry CircuitBreakerRegistry { get; }

    public CircuitBreakerOptionsBuilder AddCircuitBreaker<TOptions>()
        where TOptions : CircuitBreakerOptionsBase, new()
    {
        CircuitBreakerRegistry.Add(new TOptions());
        return this;
    }
    
    public CircuitBreakerOptionsBuilder AddCircuitBreaker<TOptions>(TOptions options)
        where TOptions : CircuitBreakerOptionsBase
    {
        CircuitBreakerRegistry.Add(options);
        return this;
    }
}