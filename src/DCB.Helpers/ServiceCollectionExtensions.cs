using Microsoft.Extensions.DependencyInjection;

namespace DCB.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterImplementationsOf<TBaseType>(
        this IServiceCollection services,
        Type? assemblyType = null)
    {
        assemblyType ??= typeof(TBaseType);
        
        assemblyType.Assembly.GetTypes()
            .Where(type => typeof(TBaseType).IsAssignableFrom(type)
                           && !type.IsInterface
                           && !type.IsAbstract)
            .ToList()
            .ForEach(implementationType => services.AddSingleton(typeof(TBaseType), implementationType));

        return services;
    }
}