using System.Net;
using Core.Settings;
using Core.Tests.ResultHandlerTests.Helpers;

namespace Core.Tests.CircuitBreakerTests.Helpers;

public sealed class ExternalServiceSettings : CircuitBreakerSettings
{
    public static ExternalServiceSettings WithDefaultTestValues => new()
    {
        FailureAllowed = 3,
        DurationOfBreak = 5.Seconds()
    };

    public ExternalServiceSettings(int failureAllowed, TimeSpan durationOfBreak):this()
    {
        FailureAllowed = failureAllowed;
        DurationOfBreak = durationOfBreak;
    }
    
    private ExternalServiceSettings()
    {
        HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);
        HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.TooManyRequests);
        
        HandleResult<ExternalServiceResponse>(x => !x.IsExchangeAvailableForToday);
        
        Name = $"CurrencyExchange-{Guid.NewGuid()}";
    }
    
    public override string Name { get; }

    public override int FailureAllowed { get; set; }

    public override TimeSpan DurationOfBreak { get; set; }
}