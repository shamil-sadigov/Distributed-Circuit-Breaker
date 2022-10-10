using Core.Storage;
using Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.Builders;

public class CircuitBreakerBuilder
{
    public CircuitBreakerBuilder(IServiceCollection services) 
        => Services = services;

    public IServiceCollection Services { get; }
    internal CircuitBreakerRegistry CircuitBreakerRegistry { get; } = new();

    public CircuitBreakerOptionsBuilder UseStorage<TStorage>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TStorage : class, ICircuitBreakerStorage
    {
        Services
            .AddScoped<ICircuitBreakerStorage, TStorage>()
            .AddScoped<ICircuitBreakerContextGetter, TStorage>()
            .AddScoped<ICircuitBreakerContextAdder, TStorage>()
            .AddScoped<ICircuitBreakerContextUpdater, TStorage>();
        
        return new CircuitBreakerOptionsBuilder(CircuitBreakerRegistry);
    }

    public CircuitBreakerBuildResult Build() => new(CircuitBreakerRegistry.GetAll());
}