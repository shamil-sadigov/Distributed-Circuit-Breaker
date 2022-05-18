using DCB.Extensions.Mongo;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DCP.Extensions.Mongo.Tests.Helpers;

public sealed class MongoOptionsProvider : IAsyncLifetime
{
    private const string ConfigFileName = "test-config.json";

    public MongoClient MongoClient { get; private set; } = null!;
    public CircuitBreakerDbOptions Options { get; set; } = null!;

    public Task InitializeAsync()
    {
        var connectionString = GetConnectionStringFromConfiguration();
        MongoClient = new MongoClient(connectionString);
        Options = new CircuitBreakerDbOptions("DistributedCircuitBreakerTestDB", "CircuitBreakers");
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await MongoClient.DropDatabaseAsync(Options.DatabaseName);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(ConfigFileName)
            .Build();

        var connectionString = config.GetConnectionString("Default");

        if (connectionString is null)
            throw new InvalidOperationException(
                $"Connection string is not provided in configuration file '{ConfigFileName}'");

        return connectionString;
    }
}