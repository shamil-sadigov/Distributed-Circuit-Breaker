namespace DCB.Core.Tests.CircuitBreakerTests.Helpers;

public class CurrencyExchangeResponse
{
    public bool IsExchangeAvailableForToday { get; set; }
    
    // image other properties...

    public static readonly CurrencyExchangeResponse Successful = new()
    {
        IsExchangeAvailableForToday = true
    };
    
    public static readonly Task<CurrencyExchangeResponse> SuccessfulResult = Task.FromResult(Successful);
    
    public static readonly CurrencyExchangeResponse Unsuccessful = new()
    {
        IsExchangeAvailableForToday = false
    };
    
    public static readonly Task<CurrencyExchangeResponse> UnsuccessfulResult = Task.FromResult(Unsuccessful);

}