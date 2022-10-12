using Core;
using Core.StateHandlers;
using Extensions.Microsoft.DependencyInjection.Registrations;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.Microsoft.DependencyInjection;

// TODO: Revise service lifetimes

public static class DistributedCircuitBreakerRegistrationExtensions
{
    public static IServiceCollection AddDistributedCircuitBreaker(
        this IServiceCollection services,
        Action<CircuitBreakerStorageRegistration> configure)
    {
        CircuitBreakerPolicyRegistration policyRegistration = new(services);
        CircuitBreakerStorageRegistration storageRegistration = new (services, policyRegistration);
        
        configure(storageRegistration);
        
        services.AddTransient(typeof(ICircuitBreaker<>), typeof(CircuitBreaker<>));

        services.RegisterImplementationsOf<ICircuitBreakerStateHandler>();

        services.AddScoped<CircuitBreakerStateHandlerProvider>();
        services.AddSingleton<ISystemClock, SystemClock>();
        
        return services;
    }
}