using AutoMapper;
using Core;
using FluentAssertions;
using Storage.Redis;

namespace Registration.Mongo.Tests;

public class MappingTests
{
    private readonly Mapper _mapper;

    public MappingTests()
    {
        _mapper = new Mapper(new MapperConfiguration(x => x.AddMaps(typeof(DataModelProfile))));
    }

    [Fact]
    public void Snapshot_is_correctly_mapped_to_data_model()
    {
        var snapshot = new CircuitBreakerSnapshot("CircuitBreakerName", 5, DateTime.UtcNow);

        var dataModel = _mapper.Map<CircuitBreakerDataModel>(snapshot);

        dataModel.Should().BeEquivalentTo(snapshot);
    }

    [Fact]
    public void Datamodel_is_correctly_mapped_to_snapshot()
    {
        var dataModel = new CircuitBreakerDataModel
        {
            Name = "CircuitBreakerName",
            FailedTimes = 5,
            LastTimeFailed = DateTime.UtcNow
        };

        var snapshot = _mapper.Map<CircuitBreakerSnapshot>(dataModel);

        snapshot.Should().BeEquivalentTo(dataModel);
    }
}