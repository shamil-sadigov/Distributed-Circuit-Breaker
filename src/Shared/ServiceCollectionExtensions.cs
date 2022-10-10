using Microsoft.Extensions.DependencyInjection;

namespace Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterImplementationsOf<TBaseType>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
        Type? assemblyType = null)
    {
        assemblyType ??= typeof(TBaseType);
        
        assemblyType.Assembly.GetTypes()
            .Where(type => typeof(TBaseType).IsAssignableFrom(type)
                           && !type.IsInterface
                           && !type.IsAbstract)
            .ToList()
            .ForEach(implementationType => 
                RegisterImplementation<TBaseType>(services, serviceLifetime, implementationType));

        return services;
    }


    // TODO: Maybe better register via ServiceDescriptor instead of doing switch case
    private static void RegisterImplementation<TBaseType>(
        IServiceCollection services, 
        ServiceLifetime serviceLifetime,
        Type implementationType)
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton(typeof(TBaseType), implementationType);
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped(typeof(TBaseType), implementationType);
                break;
            case ServiceLifetime.Transient:
                services.AddTransient(typeof(TBaseType), implementationType);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(serviceLifetime), serviceLifetime, null);
        }
    }
}