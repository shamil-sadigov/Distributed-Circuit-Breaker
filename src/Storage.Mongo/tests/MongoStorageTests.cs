using AutoMapper;
using Core;
using Core.Storage;
using FluentAssertions;
using FluentAssertions.Extensions;
using MongoDB.Driver;
using Registration.Mongo.Tests.Helpers;
using Storage.Mongo;

namespace Registration.Mongo.Tests;

// TODO: Run Mongo in docker instead of relying on local instance
// See => https://github.com/HofmeisterAn/dotnet-testcontainers

public class MongoStorageTests : IClassFixture<MongoOptionsProvider>
{
    private readonly MongoDbOptions _dbOptions;
    private readonly Mapper _mapper;
    private readonly MongoClient _mongoClient;

    public MongoStorageTests(MongoOptionsProvider mongoOptionsProvider)
    {
        _mongoClient = mongoOptionsProvider.MongoClient;
        _dbOptions = mongoOptionsProvider.Options;
        _mapper = new Mapper(new MapperConfiguration(x => x.AddMaps(typeof(DataModelProfile))));
    }

    [Fact]
    public async Task Should_return_saved_snapshot()
    {
        // Arrange
        var circuitBreakerName = $"CircuitBreakerName-{Guid.NewGuid()}";
        var snapshot = new CircuitBreakerSnapshot(circuitBreakerName, 5, DateTime.UtcNow);
        var sut = new MongoStorage(_mongoClient, _dbOptions, _mapper);

        // Act
        await sut.SaveAsync(snapshot, CancellationToken.None);

        // Assert
        var foundSnapshot = await sut.GetAsync(circuitBreakerName, CancellationToken.None);
        
        foundSnapshot.Should().BeEquivalentTo(snapshot);
    }

    [Fact]
    public async Task Should_return_updated_snapshot()
    {
        // Arrange
        var circuitBreakerName = $"CircuitBreakerName-{Guid.NewGuid()}";
        
        var snapshot = new CircuitBreakerSnapshot(circuitBreakerName, FailedTimes:5, LastTimeFailed: DateTime.UtcNow);
        
        await SaveSnapshotAsync(snapshot);
        
        var sut = new MongoStorage(_mongoClient, _dbOptions, _mapper);
        
        var foundSnapshot = await sut.GetAsync(circuitBreakerName, CancellationToken.None);

        // Act
        var updatedSnapshot = new CircuitBreakerSnapshot(
            circuitBreakerName, FailedTimes: 999, LastTimeFailed: DateTime.UtcNow + 10.Seconds());
        await sut.SaveAsync(updatedSnapshot, CancellationToken.None);

        // Assert
        foundSnapshot = await sut.GetAsync(circuitBreakerName, CancellationToken.None);
        foundSnapshot.Should().BeEquivalentTo(updatedSnapshot);
    }


    private async Task SaveSnapshotAsync(CircuitBreakerSnapshot snapshot)
    {
        var sut = new MongoStorage(_mongoClient, _dbOptions, _mapper);
        await sut.SaveAsync(snapshot, CancellationToken.None);
    }
}