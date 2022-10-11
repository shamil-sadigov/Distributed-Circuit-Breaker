using AutoMapper;
using Core;

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