using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Storage.Mongo;

namespace Registration.Mongo.Tests.Helpers;

public sealed class MongoOptionsProvider : IAsyncLifetime
{
    private const string ConfigFileName = "test-config.json";

    public MongoClient MongoClient { get; private set; } = null!;
    public MongoDbOptions Options { get; set; } = null!;

    public Task InitializeAsync()
    {
        var connectionString = GetConnectionStringFromConfiguration();
        MongoClient = new MongoClient(connectionString);
        Options = new MongoDbOptions("DistributedCircuitBreakerTestDB", "CircuitBreakers");
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