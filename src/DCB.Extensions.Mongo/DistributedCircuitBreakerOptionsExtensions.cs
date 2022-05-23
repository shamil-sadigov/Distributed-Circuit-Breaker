using DCB.Extensions.Builders;
using Microsoft.Extensions.DependencyInjection;
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
        
        options.Validate();
        
        builder.Services
            .AddSingleton(_ => options)
            .AddSingleton(sp =>
            {
                var dbOptions = sp.GetRequiredService<CircuitBreakerDbOptions>();
                return new MongoClient(dbOptions.ConnectionString);
            })
            .AddAutoMapper(typeof(DataModelProfile));

        return builder.UseStorage<MongoStorage>();
    }
}