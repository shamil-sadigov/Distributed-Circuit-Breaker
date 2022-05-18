using AutoMapper;
using DCB.Core.CircuitBreakers.States;

namespace DCB.Extensions.SqlServer;

public class DataModelMapper:Profile
{
    // TODO: Add unit tests that checks that mapper workds correctly
    public DataModelMapper()
    {
        CreateMap<CircuitBreakerContextSnapshot, CircuitBreakerDataModel>();
    }
}