using DCB.Client.WebApi.Dto;

namespace DCB.Client.WebApi.CircuitBreakerOptions;

public sealed class EventStoreCircuitBreakerOptions:Core.CircuitBreakerOption.CircuitBreakerOptions
{
    public override string Name => "RemoteLogService";
    public override int FailureAllowedBeforeBreaking => 5;
    public override TimeSpan DurationOfBreak => TimeSpan.FromSeconds(5);

    public EventStoreCircuitBreakerOptions()
    {
        HandleException<EventStoreConnectionException>(x => x.FailureReason == EventStoreFailureReason.Overwhelmed);
        HandleException<EventStoreConnectionException>(x => x.FailureReason == EventStoreFailureReason.Unavailable);
        
        HandleResult<SavedLogResult>(x => !x.IsLogSaved);
    }
}

public class EventStoreConnectionException:Exception
{
    public EventStoreFailureReason FailureReason { get; }

    public EventStoreConnectionException(EventStoreFailureReason failureReason)
    {
        FailureReason = failureReason;
    }
}

public enum EventStoreFailureReason
{
    Unknown,
    Unavailable,
    Overwhelmed,
    Unauthorized
}