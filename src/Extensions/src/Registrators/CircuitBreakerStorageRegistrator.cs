using Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Registration.Registrators;

public class CircuitBreakerStorageRegistrator
{
    private readonly CircuitBreakerOptionsRegistrator _optionsRegistrator;
    public CircuitBreakerStorageRegistrator(
        IServiceCollection services, 
        CircuitBreakerOptionsRegistrator optionsRegistrator)
    {
        _optionsRegistrator = optionsRegistrator;
        Services = services;
    }

    public IServiceCollection Services { get; }

    public CircuitBreakerOptionsRegistrator UseStorage<TStorage>()
        where TStorage : class, ICircuitBreakerStorage
    {
        Services
            .AddSingleton<ICircuitBreakerStorage, TStorage>()
            .AddSingleton<ICircuitBreakerContextGetter, TStorage>()
            .AddSingleton<ICircuitBreakerContextAdder, TStorage>()
            .AddSingleton<ICircuitBreakerContextUpdater, TStorage>();
        
        return _optionsRegistrator;
    }
}