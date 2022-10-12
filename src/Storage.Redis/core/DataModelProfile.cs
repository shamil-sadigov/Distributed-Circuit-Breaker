using AutoMapper;
using Core;
using Core.Storage;

namespace Storage.Redis;

public sealed class DataModelProfile : Profile
{
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerSnapshot, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}