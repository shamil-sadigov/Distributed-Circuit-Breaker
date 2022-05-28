using System.Net;
using DCB.Core.CircuitBreakerOption;
using DCB.Core.Tests.ResultHandlerTests.Helpers;
using FluentAssertions.Extensions;

namespace DCB.Core.Tests.CircuitBreakerTests.Helpers;

public class CurrencyExchangeServiceTestOptions : CircuitBreakerOptions
{
    // Defaults values
    public const int DefaultFailureAllowedBeforeBreaking = 5;
    public readonly TimeSpan DefaultDurationOfBreak = 3.Seconds();
    
    // Custom values
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
        => _failureAllowedBeforeBreaking ?? DefaultFailureAllowedBeforeBreaking;

    public override TimeSpan DurationOfBreak
        => _durationOfBreak ?? DefaultDurationOfBreak;
}