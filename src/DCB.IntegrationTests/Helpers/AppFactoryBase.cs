using DCB.Extensions.Mongo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DCB.IntegrationTests.Helpers;

public class AppFactoryBase<TEntryPoint>:WebApplicationFactory<TEntryPoint> , IAsyncLifetime where TEntryPoint : class
{
    private const string ConfigFileName = "test-config.json";

    public MongoClient MongoClient { get; private set; } = null!;
    public CircuitBreakerDbOptions Options { get; set; } = null!;


    public AppFactoryBase()
    {
        var connectionString = GetConnectionStringFromConfiguration();
        MongoClient = new MongoClient(connectionString);
        Options = new CircuitBreakerDbOptions("DistributedCircuitBreakerTestDB", "CircuitBreakers");
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public TService GetService<TService>() where TService : notnull
    {
        return Services.GetRequiredService<TService>();
    }
  
    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
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

   
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var service = services.FirstOrDefault(x => x.ServiceType == typeof(MongoClient));
            services.Remove(service);
            services.AddSingleton(MongoClient);
        });
    }
}