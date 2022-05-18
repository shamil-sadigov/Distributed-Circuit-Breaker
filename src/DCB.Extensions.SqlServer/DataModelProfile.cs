using AutoMapper;
using DCB.Core.CircuitBreakers.Context;

namespace DCB.Extensions.SqlServer;

public class DataModelProfile:Profile
{
    // TODO: Add unit tests that checks that mapper workds correctly
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerContextSnapshot, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}