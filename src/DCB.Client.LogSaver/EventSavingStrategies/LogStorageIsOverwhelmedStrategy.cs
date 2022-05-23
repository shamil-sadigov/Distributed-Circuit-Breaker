using DCB.Client.WebApi.CircuitBreakerOptions;
using DCB.Client.WebApi.Dto;

namespace DCB.Client.WebApi.EventSavingStrategies;

public class LogStorageIsOverwhelmedStrategy : ILogSavingStrategy
{
    public Task<SavedLogResult> SaveLogAsync(string logMessage)
    {
        throw new EventStoreConnectionException(EventStoreFailureReason.Overwhelmed);
    }
}