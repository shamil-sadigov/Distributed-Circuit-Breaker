using FunctionalTests.WebAppConfiguration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using StackExchange.Redis;
using Storage.Redis;

namespace FunctionalTests.Helpers;

public sealed class ServerFactory: IAsyncDisposable
{
    private const string ConfigFileName = "test-config.json";
    private RedisOptions RedisOptions { get; }
    
    public ServerFactory()
    {
        RedisOptions = new RedisOptions()
        {
            ConnectionString = GetConnectionStringFromConfiguration()
        };
    }
  
    public async ValueTask DisposeAsync()
    {
        var connection = await ConnectionMultiplexer.ConnectAsync(RedisOptions.ConnectionString);
        connection.GetServers().ForEach(x=> x.FlushAllDatabases());
    }

    public async Task<TestServer> CreateRunningServerAsync()
    {
        var server = new TestServer(
            new WebHostBuilder()
                .UseStartup(_ => new TestStartup(RedisOptions)));
        
        var response = await server.CreateRequest("health").GetAsync();
        response.EnsureSuccessStatusCode();
        return server;
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