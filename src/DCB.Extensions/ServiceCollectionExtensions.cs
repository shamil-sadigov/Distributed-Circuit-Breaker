using DCB.Core;
using DCB.Extensions.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace DCB.Extensions;

public static class ServiceCollectionExtensions
{
    // TODO: Test it 
    public static IServiceCollection AddDistributedCircuitBreaker(
        this IServiceCollection services, 
        Action<CircuitBreakerBuilder> configBuilder)
    {
        // configure here

        var builder = new CircuitBreakerBuilder(services);

        configBuilder(builder);
        
        // TODO: Maybe we should rename it
        CircuitBreakerBuildResult buildResult = builder.Build();
        
        services.AddSingleton(buildResult);
        
        foreach (var option in buildResult.CircuitBreakerOptions) 
            services.AddTransient(option.GetType());
        
        return services;
    }
}