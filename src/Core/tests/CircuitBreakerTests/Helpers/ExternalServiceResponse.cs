namespace Core.Tests.CircuitBreakerTests.Helpers;

public class ExternalServiceResponse
{
    public bool IsExchangeAvailableForToday { get; set; }

    private static readonly ExternalServiceResponse Successful = new()
    {
        IsExchangeAvailableForToday = true
    };

    private static readonly ExternalServiceResponse Unsuccessful = new()
    {
        IsExchangeAvailableForToday = false
    };
    
    public static readonly Task<ExternalServiceResponse> SuccessfulResult = Task.FromResult(Successful);
    
    public static readonly Task<ExternalServiceResponse> UnsuccessfulResult = Task.FromResult(Unsuccessful);

}