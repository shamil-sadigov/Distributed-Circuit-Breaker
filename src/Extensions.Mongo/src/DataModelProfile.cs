using AutoMapper;
using Core.CircuitBreakers.Context;

namespace Extensions.Mongo;

public class DataModelProfile : Profile
{
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerContextSnapshot, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}