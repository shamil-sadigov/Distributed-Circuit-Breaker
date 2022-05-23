using DCB.Extensions.Builders;
using DCB.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DCB.Extensions.Mongo;

// TODO: Test it
public static class DistributedCircuitBreakerOptionsExtensions
{
    public static CircuitBreakerOptionsBuilder UseMongo(
        this CircuitBreakerBuilder builder,
        Action<CircuitBreakerDbOptions> configureDbOptions)
    {
        if (configureDbOptions is null)
            throw new ArgumentNullException(nameof(configureDbOptions));

        var options = new CircuitBreakerDbOptions("DistributedCircuitBreaker", "CircuitBreakers");

        configureDbOptions(options);
        
        builder.Services
            .Configure(configureDbOptions)
            .AddSingleton(sp =>
            {
                var dbOptions = sp.GetRequiredService<IOptions<CircuitBreakerDbOptions>>();
                return new MongoClient(dbOptions.Value.ConnectionString);
            })
            .AddAutoMapper(typeof(DataModelProfile));

        return builder.UseStorage<MongoStorage>();
    }
}