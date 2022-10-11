using Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.Microsoft.DependencyInjection.Registrations;

public class CircuitBreakerStorageRegistration
{
    private readonly CircuitBreakerSettingsRegistration _settingsRegistration;
    public CircuitBreakerStorageRegistration(
        IServiceCollection services, 
        CircuitBreakerSettingsRegistration settingsRegistration)
    {
        _settingsRegistration = settingsRegistration;
        Services = services;
    }

    public IServiceCollection Services { get; }

    public CircuitBreakerSettingsRegistration UseStorage<TStorage>()
        where TStorage : class, ICircuitBreakerStorage
    {
        Services.AddSingleton<ICircuitBreakerStorage, TStorage>();

        return _settingsRegistration;
    }
}