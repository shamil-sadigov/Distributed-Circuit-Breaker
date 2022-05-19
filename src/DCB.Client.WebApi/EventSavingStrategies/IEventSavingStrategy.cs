using DCB.Client.WebApi.Dtos;

namespace DCB.Client.WebApi.EventSavingStrategies;

public interface IEventSavingStrategy
{
    Task<SentEventResult> SendEventAsync(string eventMessage);
}