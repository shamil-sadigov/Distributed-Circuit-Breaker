using AutoMapper;
using DCB.Core.CircuitBreakers.Context;
using DCB.Extensions.SqlServer.Tests.Helpers;
using DotNet.Testcontainers.Containers.Modules.Databases;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Extensions;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace DCB.Extensions.SqlServer.Tests;

// TODO: Run SqlServer in docker instead of relying on local instance
// See => https://github.com/HofmeisterAn/dotnet-testcontainers

public class SqlServerStorageTests:IClassFixture<DbContextProvider>
{
    private readonly DbContextProvider _dbContextProvider;
    private readonly Mapper _mapper;

    public SqlServerStorageTests(DbContextProvider dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
        _mapper = new Mapper(new MapperConfiguration(x => x.AddMaps(typeof(DataModelProfile))));
    }

    [Fact]
    public async Task Can_get_previously_added_snapshot()
    {
        // Arrange
        var circuitBreakerName = $"CircuitBreakerName-{Guid.NewGuid()}";
        var snapshot = CreateSampleSnapshot(circuitBreakerName);
        var sut = new SqlServerStorage(_dbContextProvider.Context, _mapper);

        // Act
        await sut.AddAsync(snapshot, CancellationToken.None);
        
        // Assert
        var foundSnapshot = await sut.GetAsync(circuitBreakerName, CancellationToken.None);
        foundSnapshot.Should().BeEquivalentTo(snapshot);
    }

    
    [Fact]
    public async Task Can_get_previously_updated_snapshot()
    {
        // Arrange
        var circuitBreakerName = $"CircuitBreakerName-{Guid.NewGuid()}";
        await SeedCircuitBreakerAsync(circuitBreakerName);
        var sut = new SqlServerStorage(_dbContextProvider.Context, _mapper);
        var foundSnapshot = await sut.GetAsync(circuitBreakerName, CancellationToken.None);
        
        // Act
        var modifiedSnapshot = ModifySnapshot(foundSnapshot!);
        await sut.UpdateAsync(modifiedSnapshot, CancellationToken.None);
        
        // Assert
        foundSnapshot = await sut.GetAsync(circuitBreakerName, CancellationToken.None);
        foundSnapshot.Should().BeEquivalentTo(modifiedSnapshot);
    }

    private static CircuitBreakerContextSnapshot ModifySnapshot(CircuitBreakerContextSnapshot foundSnapshot) =>
        foundSnapshot with
        {
            FailedCount = 1000,
            FailureAllowedBeforeBreaking = 1000,
            IsCircuitBreakerClosed = false,
            LastTimeStateChanged = DateTime.UtcNow - 10.Minutes(),
            TransitionDateToHalfOpenState = DateTime.Now + 10.Minutes(),
            DurationOfBreak = 20.Minutes(),
        };

    private async Task SeedCircuitBreakerAsync(string circuitBreakerName)
    {
        var snapshot = CreateSampleSnapshot(circuitBreakerName);
        var sut = new SqlServerStorage(_dbContextProvider.Context, _mapper);
        await sut.AddAsync(snapshot, CancellationToken.None);
    }

    private static CircuitBreakerContextSnapshot CreateSampleSnapshot(string circuitBreakerName) =>
        new
        (
            Name: circuitBreakerName,
            FailureAllowedBeforeBreaking: 5,
            FailedCount: 5,
            IsCircuitBreakerClosed: true,
            TransitionDateToHalfOpenState: DateTime.UtcNow + 10.Seconds(),
            LastTimeStateChanged: DateTime.UtcNow - 10.Seconds(),
            DurationOfBreak: 20.Seconds()
        );
}