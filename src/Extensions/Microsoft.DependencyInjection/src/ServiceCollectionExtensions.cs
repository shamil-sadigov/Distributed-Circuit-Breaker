using Core;
using Core.StateHandlers;
using Extensions.Microsoft.DependencyInjection.Registrations;
using Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.Microsoft.DependencyInjection;

public static class ServiceCollectionExtensions
{
    // TODO: Revise service lifetimes, they seem to be unreasonable
    
    public static IServiceCollection AddDistributedCircuitBreaker(
        this IServiceCollection services,
        Action<CircuitBreakerStorageRegistration> configure)
    {
        CircuitBreakerSettingsRegistration settingsRegistration = new(services);
        CircuitBreakerStorageRegistration storageRegistration = new (services, settingsRegistration);
        
        configure(storageRegistration);
        
        services.AddTransient(typeof(ICircuitBreaker<>), typeof(CircuitBreaker<>));

        services.RegisterImplementationsOf<ICircuitBreakerStateHandler>();

        services.AddScoped<CircuitBreakerStateHandlerProvider>();
        services.AddSingleton<ISystemClock, SystemClock>();
        
        return services;
    }
}