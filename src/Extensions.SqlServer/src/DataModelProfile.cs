using AutoMapper;
using Core.CircuitBreakers;

namespace Registration.SqlServer;

// TODO: Add test to check mapping

public class DataModelProfile : Profile
{
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerSnapshot, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}