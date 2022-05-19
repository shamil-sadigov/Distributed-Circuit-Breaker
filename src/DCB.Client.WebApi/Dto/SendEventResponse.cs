using DCB.Client.WebApi.CircuitBreakerOptions;

namespace DCB.Client.WebApi.Dto;

public class SendEventResponse
{
    public bool Succeeded { get; set; }
    public EventStoreFailureReason? FailureReason { get; set; }

    public static SendEventResponse Successful = new()
    {
        Succeeded = true
    };
    
    public static SendEventResponse Failed(EventStoreFailureReason reason) => new()
    {
        Succeeded = false,
        FailureReason = reason
    };

}