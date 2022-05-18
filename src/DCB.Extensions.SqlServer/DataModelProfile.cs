using AutoMapper;
using DCB.Core.CircuitBreakers.Context;

namespace DCB.Extensions.SqlServer;

public class DataModelProfile:Profile
{
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerContextSnapshot, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}