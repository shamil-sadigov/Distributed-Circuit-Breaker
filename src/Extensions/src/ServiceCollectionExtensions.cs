using Core;
using Core.CircuitBreakers;
using Core.CircuitBreakers.StateHandlers;
using Extensions.Builders;
using Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedCircuitBreaker(
        this IServiceCollection services,
        Action<CircuitBreakerBuilder> configBuilder)
    {
        // configure here

        var builder = new CircuitBreakerBuilder(services);

        configBuilder(builder);

        // TODO: Maybe we should rename it
        var buildResult = builder.Build();

        services.AddSingleton(buildResult);

        foreach (var option in buildResult.CircuitBreakerOptions)
            services.AddSingleton(option.GetType(), _ => option);
        
        services.AddTransient(typeof(ICircuitBreaker<>), typeof(CircuitBreaker<>));

        services.RegisterImplementationsOf<ICircuitBreakerStateHandler>();

        services.AddScoped<CircuitBreakerStateHandlerProvider>();
        services.AddSingleton<ISystemClock, SystemClock>();
        
        return services;
    }
}