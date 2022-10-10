using AutoMapper;
using Core.CircuitBreakers.Context;

namespace Registration.Mongo;

public class DataModelProfile : Profile
{
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerState, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}