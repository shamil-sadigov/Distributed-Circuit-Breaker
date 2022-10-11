using Extensions.Microsoft.DependencyInjection.Registrations;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Storage.Mongo.Extensions.Microsoft.DependencyInjection;

// TODO: Test it
public static class CircuitBreakerStorageRegistrationExtensions
{
    public static CircuitBreakerSettingsRegistration UseMongo(
        this CircuitBreakerStorageRegistration storageRegistration,
        Action<MongoDbOptions> configure)
    {
        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        var mongoOptions = new MongoDbOptions("DistributedCircuitBreaker", "CircuitBreakers");

        configure(mongoOptions);
        
        mongoOptions.Validate();
        
        storageRegistration.Services
            .AddSingleton(_ => mongoOptions)
            .AddSingleton(sp =>
            {
                var dbOptions = sp.GetRequiredService<MongoDbOptions>();
                return new MongoClient(dbOptions.ConnectionString);
            })
            .AddAutoMapper(typeof(DataModelProfile));

        return storageRegistration.UseStorage<MongoStorage>();
    }
}