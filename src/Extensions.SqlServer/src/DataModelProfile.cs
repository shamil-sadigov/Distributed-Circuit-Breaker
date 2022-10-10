using AutoMapper;
using Core.CircuitBreakers.Context;

namespace Extensions.SqlServer;

public class DataModelProfile : Profile
{
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerContextSnapshot, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}