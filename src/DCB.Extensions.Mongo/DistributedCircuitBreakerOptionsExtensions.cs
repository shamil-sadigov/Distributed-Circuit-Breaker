using DCB.Extensions.Builders;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DCB.Extensions.Mongo;

// TODO: Test it
public static class DistributedCircuitBreakerOptionsExtensions
{
    public static CircuitBreakerOptionsBuilder UseMongo(
        this CircuitBreakerBuilder builder,
        string connectionString)
    {
        builder.Services
            .AddSingleton(new CircuitBreakerDbOptions("DistributedCircuitBreaker", "CircuitBreakers"))
            .AddSingleton(new MongoClient(connectionString))
            .AddAutoMapper(typeof(DataModelProfile));

        return builder.UseStorage<MongoStorage>();
    }
}