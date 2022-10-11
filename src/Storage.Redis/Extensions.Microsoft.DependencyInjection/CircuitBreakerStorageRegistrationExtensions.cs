using Extensions.Microsoft.DependencyInjection.Registrations;
using Microsoft.Extensions.DependencyInjection;

namespace Storage.Redis.Extensions.Microsoft.DependencyInjection;

// TODO: Test it
public static class CircuitBreakerStorageRegistrationExtensions
{
    public static CircuitBreakerSettingsRegistration UseMongo(
        this CircuitBreakerStorageRegistration storageRegistration,
        Action<RedisDbOptions> configure)
    {
        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        var redisOptions = new RedisDbOptions();

        configure(redisOptions);
        
        redisOptions.Validate();
        
        storageRegistration.Services
            .AddSingleton(_ => redisOptions)
            .AddAutoMapper(typeof(DataModelProfile));

        return storageRegistration.UseStorage<RedisStorage>();
    }
}