using AutoMapper;
using Core.CircuitBreakers.Context;
using FluentAssertions;
using FluentAssertions.Extensions;

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
        var snapshot = new CircuitBreakerState
        (
            "CircuitBreakerName",
            5,
            5,
            true,
            DateTime.UtcNow + 10.Seconds(),
            DateTime.UtcNow - 10.Seconds(),
            20.Seconds()
        );

        var dataModel = _mapper.Map<CircuitBreakerDataModel>(snapshot);

        dataModel.Should().BeEquivalentTo(snapshot);
    }

    [Fact]
    public void Datamodel_is_correctly_mapped_to_snapshot()
    {
        var dataModel = new CircuitBreakerDataModel
        {
            Name = "CircuitBreakerName",
            FailureAllowedBeforeBreaking = 5,
            FailedCount = 5,
            TransitionDateToHalfOpenState = DateTime.UtcNow + 10.Seconds(),
            LastTimeStateChanged = DateTime.UtcNow - 10.Seconds(),
            DurationOfBreak = 20.Seconds()
        };

        var snapshot = _mapper.Map<CircuitBreakerState>(dataModel);

        snapshot.Should().BeEquivalentTo(dataModel);
    }
}