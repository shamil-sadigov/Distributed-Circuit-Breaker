namespace DCB.Client.Shared.LogSavingStrategies;

public class LogStorageIsOverwhelmedStrategy : ILogSavingStrategy
{
    public Task<SavedLogResult> SaveLogAsync(string logMessage)
    {
        throw new EventStoreConnectionException(LogStorageFailureReason.Overwhelmed);
    }
}