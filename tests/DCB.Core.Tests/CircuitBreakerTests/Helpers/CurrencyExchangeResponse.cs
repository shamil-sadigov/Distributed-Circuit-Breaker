namespace DCB.Core.Tests.CircuitBreakerTests.Helpers;

public class CurrencyExchangeResponse
{
    public bool IsExchangeAvailableForToday { get; set; }

    private static readonly CurrencyExchangeResponse Successful = new()
    {
        IsExchangeAvailableForToday = true
    };

    private static readonly CurrencyExchangeResponse Unsuccessful = new()
    {
        IsExchangeAvailableForToday = false
    };
    
    public static readonly Task<CurrencyExchangeResponse> SuccessfulResult = Task.FromResult(Successful);
    
    public static readonly Task<CurrencyExchangeResponse> UnsuccessfulResult = Task.FromResult(Unsuccessful);

}