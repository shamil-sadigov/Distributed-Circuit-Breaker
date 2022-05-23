using DCB.Client.WebApi.CircuitBreakerOptions;
using DCB.Client.WebApi.Dto;

namespace DCB.Client.WebApi.EventSavingStrategies;

public class ClientUnauthorizedStrategy : IEventSavingStrategy
{
    public Task<SentEventResult> SaveEventAsync(string eventMessage)
    {
        throw new EventStoreConnectionException(EventStoreFailureReason.Unauthorized);
    }
}