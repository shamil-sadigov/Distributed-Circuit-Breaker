using Core;
using Core.StateHandlers;
using Helpers;
using Microsoft.Extensions.DependencyInjection;
using Registration.Registrators;

namespace Registration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedCircuitBreaker(
        this IServiceCollection services,
        Action<CircuitBreakerStorageRegistrator> configure)
    {
        CircuitBreakerOptionsRegistrator optionsRegistrator = new(services);
        CircuitBreakerStorageRegistrator storageRegistrator = new (services, optionsRegistrator);
        
        configure(storageRegistrator);
        
        services.AddTransient(typeof(ICircuitBreaker<>), typeof(CircuitBreaker<>));

        services.RegisterImplementationsOf<ICircuitBreakerStateHandler>();

        services.AddScoped<CircuitBreakerStateHandlerProvider>();
        services.AddSingleton<ISystemClock, SystemClock>();
        
        return services;
    }
}