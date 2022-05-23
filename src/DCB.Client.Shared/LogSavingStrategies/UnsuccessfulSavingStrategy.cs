namespace DCB.Client.Shared.LogSavingStrategies;

public class UnsuccessfulSavingStrategy:ILogSavingStrategy
{
    private readonly LogStorageFailureReason _reason;

    public UnsuccessfulSavingStrategy(LogStorageFailureReason reason)
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