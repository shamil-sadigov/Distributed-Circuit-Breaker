using System.Net;
using DCB.Core.CircuitBreakerOption;
using DCB.Core.Tests.ResultHandlerTests.Helpers;

namespace DCB.Core.Tests.CircuitBreakerTests.Helpers;

public class CurrencyExchangeServiceTestOptions : CircuitBreakerOptions
{
    private readonly int? _failureAllowedBeforeBreaking;
    private readonly TimeSpan? _durationOfBreak;

    public CurrencyExchangeServiceTestOptions(
        int failureAllowedBeforeBreaking, 
        TimeSpan durationOfBreak):this()
    {
        _failureAllowedBeforeBreaking = failureAllowedBeforeBreaking;
        _durationOfBreak = durationOfBreak;
    }
    public CurrencyExchangeServiceTestOptions()
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