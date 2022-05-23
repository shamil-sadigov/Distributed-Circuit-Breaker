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

    public void SetStrategy(IEventSavingStrategy newStrategy)
    {
        EventSavingStrategy = newStrategy ?? throw new ArgumentNullException(nameof(newStrategy));
    }
    
    /// <summary>
    /// Sends log to remote service
    /// </summary>
    public Task<SentEventResult> SaveEventAsync(string eventMessage)
    {
        return EventSavingStrategy.SaveEventAsync(eventMessage);
    }
}