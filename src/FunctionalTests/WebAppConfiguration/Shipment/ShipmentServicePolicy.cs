using Core.Policy;

namespace FunctionalTests.WebAppConfiguration.Shipment;

public sealed class ShipmentServicePolicy:CircuitBreakerPolicy
{
    public override string Name => "Shipment-Service";
    public override int FailureAllowed { get; set; } = 10;
    public override TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(5);

    public ShipmentServicePolicy()
    {
        HandleException<ShipmentServiceException>(x => x.FailureReason == FailureReason.RateLimited);
        HandleException<ShipmentServiceException>(x => x.FailureReason == FailureReason.Unavailable);
        
        HandleResult<ShipmentResult>(x => !x.IsShipmentAccepted);

    }
}