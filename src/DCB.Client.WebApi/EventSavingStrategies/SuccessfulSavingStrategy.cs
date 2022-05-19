using DCB.Client.WebApi.Dtos;

namespace DCB.Client.WebApi.EventSavingStrategies;

public class SuccessfulSavingStrategy:IEventSavingStrategy
{
    public Task<SentEventResult> SendEventAsync(string eventMessage)
    {
        return Task.FromResult(new SentEventResult()
        {
            IsEventSent = true
        });
    }
}