namespace DCB.Client.Shared.LogSavingStrategies;

public class ClientUnauthorizedStrategy : ILogSavingStrategy
{
    public Task<SavedLogResult> SaveLogAsync(string logMessage)
    {
        throw new EventStoreConnectionException(LogStorageFailureReason.Unauthorized);
    }
}