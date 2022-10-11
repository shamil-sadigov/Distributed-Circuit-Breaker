using AutoMapper;
using Core;
using FluentAssertions;
using FluentAssertions.Extensions;
using Storage.Redis;

namespace Registration.Mongo.Tests;

// TODO: Run Mongo in docker instead of relying on local instance
// See => https://github.com/HofmeisterAn/dotnet-testcontainers

public class RedisStorageTests 
{
    private readonly Mapper _mapper;
    private readonly RedisDbOptions _dbOptions;

    public RedisStorageTests()
    {
        _mapper = new Mapper(new MapperConfiguration(x => x.AddMaps(typeof(DataModelProfile))));
        _dbOptions = new RedisDbOptions()
        {
            ConnectionString = "localhost"
        };
    }

    [Fact]
    public async Task Can_get_previously_saved_snapshot()
    {
        var circuitBreakerName = $"CircuitBreakerName-{Guid.NewGuid()}";

        // Arrange
        var snapshot = new CircuitBreakerSnapshot(circuitBreakerName, 5, DateTime.UtcNow);
        var sut = new RedisStorage(_dbOptions, _mapper);

        // Act
        await sut.SaveAsync(snapshot, CancellationToken.None);

        // Assert
        var foundSnapshot = await sut.GetAsync(circuitBreakerName, CancellationToken.None);
        
        foundSnapshot.Should().BeEquivalentTo(snapshot);
    }

    [Fact]
    public async Task Can_get_updated_snapshot()
    {
        // Arrange
        var circuitBreakerName = $"CircuitBreakerName-{Guid.NewGuid()}";
        
        var snapshot = new CircuitBreakerSnapshot(circuitBreakerName, FailedTimes:5, LastTimeFailed: DateTime.UtcNow);
        
        await SaveSnapshotAsync(snapshot);
        
        var sut = new RedisStorage(_dbOptions, _mapper);

        // Act
        var updatedSnapshot = new CircuitBreakerSnapshot(
            circuitBreakerName, FailedTimes: 999, LastTimeFailed: DateTime.UtcNow + 10.Seconds());
        
        await sut.SaveAsync(updatedSnapshot, CancellationToken.None);

        // Assert
        var foundSnapshot = await sut.GetAsync(circuitBreakerName, CancellationToken.None);
        foundSnapshot.Should().BeEquivalentTo(updatedSnapshot);
    }

    private async Task SaveSnapshotAsync(CircuitBreakerSnapshot snapshot)
    {
        var sut = new RedisStorage(_dbOptions, _mapper);
        await sut.SaveAsync(snapshot, CancellationToken.None);
    }
}