using DCB.Client.WebApi.CircuitBreakerOptions;
using DCB.Client.WebApi.Dto;

namespace DCB.Client.WebApi.EventSavingStrategies;

public class UnsuccessfulSavingStrategy:ILogSavingStrategy
{
    private readonly EventStoreFailureReason _reason;

    public UnsuccessfulSavingStrategy(EventStoreFailureReason reason)
    {
        _reason = reason;
    }
    
    public Task<SavedLogResult> SaveLogAsync(string logMessage)
    {
        return Task.FromResult(new SavedLogResult()
        {
            IsLogSaved = false
        });
    }
}