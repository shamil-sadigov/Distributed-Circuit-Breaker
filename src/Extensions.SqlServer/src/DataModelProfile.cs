using AutoMapper;
using Core;

namespace Registration.SqlServer;
    
public class DataModelProfile : Profile
{
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerSnapshot, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}