using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Storage.SqlServer.Tests.Helpers;

public sealed class DbContextProvider : IAsyncLifetime
{
    private const string ConfigFileName = "test-config.json";

    public CircuitBreakerDbContext Context { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var connectionString = GetConnectionStringFromConfiguration();
        await InitializeDbContextAsync(connectionString);
    }

    public async Task DisposeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
    }

    private async Task InitializeDbContextAsync(string? connectionString)
    {
        var ops = new DbContextOptionsBuilder<CircuitBreakerDbContext>()
            .UseSqlServer("Data Source=darwin;Initial Catalog=CircuitBreakerTests;Integrated Security=True")
            .Options;

        Context = new CircuitBreakerDbContext(ops);

        await Context.Database.EnsureCreatedAsync();

        if (!await Context.Database.CanConnectAsync())
            throw new InvalidOperationException($"Cannot connect to DB with connection string '{connectionString}'");
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