namespace DCB.Client.Shared.LogSavingStrategies;

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