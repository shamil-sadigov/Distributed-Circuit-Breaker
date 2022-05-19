using DCB.Client.WebApi.Dto;
using DCB.Client.WebApi.EventSavingStrategies;

namespace DCB.Client.WebApi;

/// <summary>
/// Sample services for saving events into some EventStore.
/// This class is for testing of CircuitBreaker purposes.
/// </summary>
public class EventStore
{
    private IEventSavingStrategy EventSavingStrategy { get; set; } = new SuccessfulSavingStrategy();
    
    /// <summary>
    /// Sends log to remote service
    /// </summary>
    public Task<SentEventResult> SendEventAsync(string eventMessage)
    {
        return EventSavingStrategy.SendEventAsync(eventMessage);
    }
}