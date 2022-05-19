using DCB.Client.WebApi.CircuitBreakerOptions;
using DCB.Client.WebApi.Dto;

namespace DCB.Client.WebApi.EventSavingStrategies;

public class UnsuccessfulSavingStrategy:IEventSavingStrategy
{
    private readonly EventStoreFailureReason _reason;

    public UnsuccessfulSavingStrategy(EventStoreFailureReason reason)
    {
        _reason = reason;
    }
    
    public Task<SentEventResult> SendEventAsync(string eventMessage)
    {
        return Task.FromResult(new SentEventResult()
        {
            IsEventSent = false
        });
    }
}