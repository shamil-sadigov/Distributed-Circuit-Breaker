using DCB.Client.WebApi.Dto;

namespace DCB.Client.WebApi.EventSavingStrategies;

public class SuccessfulSavingStrategy:ILogSavingStrategy
{
    public Task<SavedLogResult> SaveLogAsync(string logMessage)
    {
        return Task.FromResult(new SavedLogResult()
        {
            IsLogSaved = true
        });
    }
}