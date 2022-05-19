using DCB.Core.Storage;
using DCB.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace DCB.Extensions.Builders;

public class CircuitBreakerBuilder
{
    public CircuitBreakerBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
    internal CircuitBreakerRegistry CircuitBreakerRegistry { get; } = new();

    public CircuitBreakerOptionsBuilder UseStorage<TStorage>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TStorage : class, ICircuitBreakerStorage
    {
        Services.AddScoped<ICircuitBreakerStorage, TStorage>();
        
        Services.AddScoped<ICircuitBreakerContextGetter, TStorage>();
        Services.AddScoped<ICircuitBreakerContextAdder, TStorage>();
        Services.AddScoped<ICircuitBreakerContextUpdater, TStorage>();
        
        return new CircuitBreakerOptionsBuilder(CircuitBreakerRegistry);
    }

    public CircuitBreakerBuildResult Build()
    {
        return new(CircuitBreakerRegistry.GetAll());
    }
}