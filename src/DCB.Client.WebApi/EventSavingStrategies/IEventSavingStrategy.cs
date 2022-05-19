﻿using DCB.Client.WebApi.Dto;

namespace DCB.Client.WebApi.EventSavingStrategies;

public interface IEventSavingStrategy
{
    Task<SentEventResult> SendEventAsync(string eventMessage);
}