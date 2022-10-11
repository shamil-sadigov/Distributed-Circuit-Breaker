using AutoMapper;
using Storage.SqlServer.Tests.Helpers;
using Helpers.Tests;

namespace Storage.SqlServer.Tests;

// TODO: Run SqlServer in docker instead of relying on local instance
// See => https://github.com/HofmeisterAn/dotnet-testcontainers

public class SqlServerStorageTests : IClassFixture<DbContextProvider>
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
        var snapshot = SnapshotHelper.CreateSampleSnapshot(circuitBreakerName);
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
        var modifiedSnapshot = SnapshotHelper.ChangeValues(foundSnapshot!);
        await sut.UpdateAsync(modifiedSnapshot, CancellationToken.None);

        // Assert
        foundSnapshot = await sut.GetAsync(circuitBreakerName, CancellationToken.None);
        foundSnapshot.Should().BeEquivalentTo(modifiedSnapshot);
    }


    private async Task SeedCircuitBreakerAsync(string circuitBreakerName)
    {
        var snapshot = SnapshotHelper.CreateSampleSnapshot(circuitBreakerName);
        var sut = new SqlServerStorage(_dbContextProvider.Context, _mapper);
        await sut.AddAsync(snapshot, CancellationToken.None);
    }
}