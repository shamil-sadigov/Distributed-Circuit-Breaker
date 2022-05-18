using DCB.Core;
using DCB.Core.Storage;
using DCB.Extensions.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace DCB.Extensions.Builders;

public class CircuitBreakerBuilder
{
    public IServiceCollection Services { get; }
    internal CircuitBreakerRegistry CircuitBreakerRegistry { get; } = new();
    
    public CircuitBreakerBuilder(IServiceCollection services)
    {
        Services = services;
    }
    
    public CircuitBreakerOptionsBuilder UseStorage<TStorage>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TStorage: class, ICircuitBreakerStorage
    {
        Services.AddScoped<ICircuitBreakerStorage, TStorage>();
        return new CircuitBreakerOptionsBuilder(CircuitBreakerRegistry);
    }
    
    public CircuitBreakerBuildResult Build() 
        => new CircuitBreakerBuildResult(CircuitBreakerRegistry.GetAll());
}