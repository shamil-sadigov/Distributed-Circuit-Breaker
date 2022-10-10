namespace DCB.Client.Shared.LogSavingStrategies;

public class LogStorageIsNotAvailableStrategy : ILogSavingStrategy
{
    public Task<SavedLogResult> SaveLogAsync(string logMessage)
    {
        throw new EventStoreConnectionException(LogStorageFailureReason.Unavailable);
    }
}