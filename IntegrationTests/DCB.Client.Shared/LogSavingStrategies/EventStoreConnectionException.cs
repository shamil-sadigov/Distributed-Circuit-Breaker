namespace DCB.Client.Shared.LogSavingStrategies;

public class EventStoreConnectionException:Exception
{
    public LogStorageFailureReason FailureReason { get; }

    public EventStoreConnectionException(LogStorageFailureReason failureReason)
    {
        FailureReason =  failureReason;
    }
}