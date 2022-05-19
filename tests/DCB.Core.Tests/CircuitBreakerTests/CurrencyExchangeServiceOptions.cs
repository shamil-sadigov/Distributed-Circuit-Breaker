using System.Net;
using DCB.Core.CircuitBreakerOption;
using DCB.Core.Tests.ResultHandler.Helpers;

namespace DCB.Core.Tests.FunctionalTests;

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


public class CurrencyExchangeServiceOptions : CircuitBreakerOptions
{
    private readonly int? _failureAllowedBeforeBreaking;
    private readonly TimeSpan? _durationOfBreak;

    public CurrencyExchangeServiceOptions(
        int failureAllowedBeforeBreaking, 
        TimeSpan durationOfBreak):this()
    {
        _failureAllowedBeforeBreaking = failureAllowedBeforeBreaking;
        _durationOfBreak = durationOfBreak;
    }
    public CurrencyExchangeServiceOptions()
    {
        HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);
        HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.TooManyRequests);
        HandleResult<CurrencyExchangeResponse>(x => !x.IsExchangeAvailableForToday);
        Name = $"CurrencyExchange-{Guid.NewGuid()}";
    }
    
    public override string Name { get; }
    
    public override int FailureAllowedBeforeBreaking 
        => _failureAllowedBeforeBreaking ?? 5;
    
    public override TimeSpan DurationOfBreak 
        => _durationOfBreak ?? TimeSpan.FromSeconds(3);
}