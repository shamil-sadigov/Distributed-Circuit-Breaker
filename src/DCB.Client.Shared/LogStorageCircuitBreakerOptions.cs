using DCB.Client.Shared.LogSavingStrategies;
using DCB.Core.CircuitBreakerOption;

namespace DCB.Client.Shared;

public sealed class LogStorageCircuitBreakerOptions:CircuitBreakerOptions
{
    public override string Name => "RemoteLogService";
    public override int FailureAllowedBeforeBreaking => 5;
    public override TimeSpan DurationOfBreak => TimeSpan.FromSeconds(5);

    public LogStorageCircuitBreakerOptions()
    {
        HandleException<EventStoreConnectionException>(x => x.FailureReason == LogStorageFailureReason.Overwhelmed);
        HandleException<EventStoreConnectionException>(x => x.FailureReason == LogStorageFailureReason.Unavailable);
        
        HandleResult<SavedLogResult>(x => !x.IsLogSaved);
    }
}