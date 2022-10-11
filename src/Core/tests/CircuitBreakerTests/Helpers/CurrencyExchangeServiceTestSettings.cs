using System.Net;
using Core.Settings;
using Core.Tests.ResultHandlerTests.Helpers;
using FluentAssertions.Extensions;

namespace Core.Tests.CircuitBreakerTests.Helpers;

public class CurrencyExchangeServiceTestSettings : CircuitBreakerSettings
{
    // Defaults values
    public const int DefaultFailureAllowedBeforeBreaking = 5;
    public readonly TimeSpan DefaultDurationOfBreak = 3.Seconds();
    
    // Custom values
    private readonly int? _failureAllowedBeforeBreaking;
    private readonly TimeSpan? _durationOfBreak;
    
    public CurrencyExchangeServiceTestSettings(
        int failureAllowedBeforeBreaking, 
        TimeSpan durationOfBreak):this()
    {
        _failureAllowedBeforeBreaking = failureAllowedBeforeBreaking;
        _durationOfBreak = durationOfBreak;
    }
    public CurrencyExchangeServiceTestSettings()
    {
        HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);
        HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.TooManyRequests);
        
        HandleResult<CurrencyExchangeResponse>(x => !x.IsExchangeAvailableForToday);
        
        Name = $"CurrencyExchange-{Guid.NewGuid()}";
    }
    
    public override string Name { get; }
    
    public override int FailureAllowed 
        => _failureAllowedBeforeBreaking ?? DefaultFailureAllowedBeforeBreaking;

    public override TimeSpan DurationOfBreak
        => _durationOfBreak ?? DefaultDurationOfBreak;
}