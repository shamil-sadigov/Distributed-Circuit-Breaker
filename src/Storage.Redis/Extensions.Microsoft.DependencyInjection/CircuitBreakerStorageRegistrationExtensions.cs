using Extensions.Microsoft.DependencyInjection.Registrations;
using Microsoft.Extensions.DependencyInjection;

namespace Storage.Redis.Extensions.Microsoft.DependencyInjection;

// TODO: Test it
public static class CircuitBreakerStorageRegistrationExtensions
{
    public static CircuitBreakerPolicyRegistration UseRedis(
        this CircuitBreakerStorageRegistration storageRegistration,
        Action<RedisOptions> configure)
    {
        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        var redisOptions = new RedisOptions();

        configure(redisOptions);

        return UseRedis(storageRegistration, redisOptions);
    }
    
    public static CircuitBreakerPolicyRegistration UseRedis(
        this CircuitBreakerStorageRegistration storageRegistration,
        RedisOptions redisOptions)
    {
        if (redisOptions is null)
            throw new ArgumentNullException(nameof(redisOptions));
        
        redisOptions.Validate();
        
        storageRegistration.Services
            .AddSingleton(_ => redisOptions)
            .AddAutoMapper(typeof(DataModelProfile));

        return storageRegistration.UseStorage<RedisStorage>();
    }
}