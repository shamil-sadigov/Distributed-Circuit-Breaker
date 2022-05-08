using Microsoft.Extensions.DependencyInjection;

namespace DCB.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedCircuitBreaker(
        this IServiceCollection services)
    {
        // configure here
        
        return services;
    }
}