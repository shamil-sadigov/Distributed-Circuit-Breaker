﻿using AutoMapper;
using Core;

namespace Storage.Mongo;

public class DataModelProfile : Profile
{
    public DataModelProfile()
    {
        CreateMap<CircuitBreakerSnapshot, CircuitBreakerDataModel>()
            .ReverseMap();
    }
}