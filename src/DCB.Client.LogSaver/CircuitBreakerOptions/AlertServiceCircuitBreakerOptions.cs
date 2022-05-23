using System.Net;

namespace DCB.Client.WebApi.CircuitBreakerOptions;

public sealed class AlertServiceCircuitBreakerOptions:Core.CircuitBreakerOption.CircuitBreakerOptions
{
    public override string Name => "RemoteAlertService";
    public override int FailureAllowedBeforeBreaking => 5;
    public override TimeSpan DurationOfBreak => TimeSpan.FromSeconds(5);

    public AlertServiceCircuitBreakerOptions()
    {
        HandleException<HttpRequestException>(x => x.StatusCode == HttpStatusCode.TooManyRequests);
        HandleException<HttpRequestException>(x => x.StatusCode == HttpStatusCode.ServiceUnavailable);
    }
}