using DCB.Client.WebApi.Dto;

namespace DCB.Client.WebApi.EventSavingStrategies;

public class SuccessfulSavingStrategy:IEventSavingStrategy
{
    public Task<SentEventResult> SaveEventAsync(string eventMessage)
    {
        return Task.FromResult(new SentEventResult()
        {
            IsEventSent = true
        });
    }
}