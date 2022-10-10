using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Registration.Registrators;

namespace Registration.Mongo;

// TODO: Test it
public static class DistributedCircuitBreakerOptionsExtensions
{
    public static CircuitBreakerOptionsRegistrator UseMongo(
        this CircuitBreakerStorageRegistrator storageRegistrator,
        Action<MongoDbOptions> configure)
    {
        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        var mongoOptions = new MongoDbOptions("DistributedCircuitBreaker", "CircuitBreakers");

        configure(mongoOptions);
        
        mongoOptions.Validate();
        
        storageRegistrator.Services
            .AddSingleton(_ => mongoOptions)
            .AddSingleton(sp =>
            {
                var dbOptions = sp.GetRequiredService<MongoDbOptions>();
                return new MongoClient(dbOptions.ConnectionString);
            })
            .AddAutoMapper(typeof(DataModelProfile));

        return storageRegistrator.UseStorage<MongoStorage>();
    }
}