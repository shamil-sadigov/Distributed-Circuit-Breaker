using AutoMapper;
using DCB.Extensions.Mongo;
using DCB.Tests.Shared;
using DCP.Extensions.Mongo.Tests.Helpers;
using FluentAssertions;
using MongoDB.Driver;

namespace DCP.Extensions.Mongo.Tests;

// TODO: Run Mongo in docker instead of relying on local instance
// See => https://github.com/HofmeisterAn/dotnet-testcontainers

public class MongoStorageTests : IClassFixture<MongoOptionsProvider>
{
    private readonly CircuitBreakerDbOptions _dbOptions;
    private readonly Mapper _mapper;
    private readonly MongoClient _mongoClient;

    public MongoStorageTests(MongoOptionsProvider mongoOptionsProvider)
    {
        _mongoClient = mongoOptionsProvider.MongoClient;
        _dbOptions = mongoOptionsProvider.Options;
        _mapper = new Mapper(new MapperConfiguration(x => x.AddMaps(typeof(DataModelProfile))));
    }

    [Fact]
    public async Task Can_get_previously_added_snapshot()
    {
        // Arrange
        var circuitBreakerName = $"CircuitBreakerName-{Guid.NewGuid()}";
        var snapshot = SnapshotHelper.CreateSampleSnapshot(circuitBreakerName);
        var sut = new MongoStorage(_mongoClient, _dbOptions, _mapper);

        // Act
        await sut.AddAsync(snapshot, CancellationToken.None);

        // Assert
        var foundSnapshot = await sut.GetAsync(circuitBreakerName, CancellationToken.None);

        // By default Mongo saves 

        foundSnapshot.Should().BeEquivalentTo(snapshot);
    }

    [Fact]
    public async Task Can_get_previously_updated_snapshot()
    {
        // Arrange
        var circuitBreakerName = $"CircuitBreakerName-{Guid.NewGuid()}";
        await SeedCircuitBreakerAsync(circuitBreakerName);
        var sut = new MongoStorage(_mongoClient, _dbOptions, _mapper);
        var foundSnapshot = await sut.GetAsync(circuitBreakerName, CancellationToken.None);

        // Act
        var modifiedSnapshot = SnapshotHelper.ChangeValues(foundSnapshot!);
        await sut.UpdateAsync(modifiedSnapshot, CancellationToken.None);

        // Assert
        foundSnapshot = await sut.GetAsync(circuitBreakerName, CancellationToken.None);
        foundSnapshot.Should().BeEquivalentTo(modifiedSnapshot);
    }

    private async Task SeedCircuitBreakerAsync(string circuitBreakerName)
    {
        var snapshot = SnapshotHelper.CreateSampleSnapshot(circuitBreakerName);
        var sut = new MongoStorage(_mongoClient, _dbOptions, _mapper);
        await sut.AddAsync(snapshot, CancellationToken.None);
    }
}