using AutoMapper;
using Core.CircuitBreakers.Context;

namespace Registration.SqlServer;

// TODO: Add test to check mapping

public class DataModelProfile : Profile
{
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerState, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}