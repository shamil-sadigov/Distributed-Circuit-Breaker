using DCB.Client.WebApi.Dto;
using DCB.Client.WebApi.EventSavingStrategies;

namespace DCB.Client.WebApi;

/// <summary>
/// Sample services for saving events into some EventStore.
/// This class is for testing of CircuitBreaker purposes.
/// </summary>
public class LogStorage
{
    private ILogSavingStrategy LogSavingStrategy { get; set; } = new SuccessfulSavingStrategy();

    public void SetStrategy(ILogSavingStrategy newStrategy)
    {
        LogSavingStrategy = newStrategy ?? throw new ArgumentNullException(nameof(newStrategy));
    }
    
    /// <summary>
    /// Sends log to remote service
    /// </summary>
    public Task<SavedLogResult> SaveLogAsync(string logMessage)
    {
        return LogSavingStrategy.SaveLogAsync(logMessage);
    }
}