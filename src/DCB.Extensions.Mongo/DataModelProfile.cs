using AutoMapper;
using DCB.Core.CircuitBreakers.Context;

namespace DCB.Extensions.Mongo;

public class DataModelProfile : Profile
{
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerContextSnapshot, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}