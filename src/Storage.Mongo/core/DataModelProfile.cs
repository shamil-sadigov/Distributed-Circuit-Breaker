using AutoMapper;
using Core.Storage;

namespace Storage.Mongo;

public sealed class DataModelProfile : Profile
{
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerSnapshot, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}