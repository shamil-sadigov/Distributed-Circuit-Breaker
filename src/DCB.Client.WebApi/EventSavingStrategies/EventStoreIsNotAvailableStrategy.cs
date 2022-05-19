using DCB.Client.WebApi.CircuitBreakerOptions;
using DCB.Client.WebApi.Dtos;

namespace DCB.Client.WebApi.EventSavingStrategies;

public class EventStoreIsNotAvailableStrategy : IEventSavingStrategy
{
    public Task<SentEventResult> SendEventAsync(string eventMessage)
    {
        throw new EventStoreConnectionException(EventStoreFailureReason.Unavailable);
    }
}