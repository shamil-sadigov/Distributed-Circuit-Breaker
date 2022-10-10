using AutoMapper;
using Core.CircuitBreakers.Context;

namespace Registration.SqlServer;

public class DataModelProfile : Profile
{
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerContextSnapshot, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}