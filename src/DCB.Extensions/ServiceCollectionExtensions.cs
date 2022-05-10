using DCB.Core;
using DCB.Core.CircuitBreakerOptions;
using Microsoft.Extensions.DependencyInjection;

namespace DCB.Extensions;

public static class ServiceCollectionExtensions
{
    // TODO: Test it 
    public static IServiceCollection AddDistributedCircuitBreaker(
        this IServiceCollection services, 
        Action<CircuitBreakerContextBuilder> configBuilder)
    {
        // configure here

        var builder = new CircuitBreakerContextBuilder();

        configBuilder(builder);
        
        CircuitBreakerContext context = builder.Build();
        
        services.AddSingleton(context);
        
        foreach (var option in context.CircuitBreakerOptions)
        {
            var typedOptions = typeof(CircuitBreakerOptions<>)
                .MakeGenericType(option.GetType());
            
            services.AddTransient(typedOptions, option.GetType());
        }

        services.AddScoped(typeof(ICircuitBreakerStorage), context.StorageType);
        
        return services;
    }
}

