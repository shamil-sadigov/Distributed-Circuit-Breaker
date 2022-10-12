﻿using Core.Settings;

namespace IntegrationTests.WebAppConfiguration.Prometheus;

public sealed class ShipmentServiceSettings:CircuitBreakerSettings
{
    public override string Name => "Shipment-Service";
    public override int FailureAllowed { get; set; } = 10;
    public override TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(5);

    public ShipmentServiceSettings()
    {
        HandleException<ShipmentServiceConnectionException>(x => x.FailureReason == FailureReason.RateLimited);
        HandleException<ShipmentServiceConnectionException>(x => x.FailureReason == FailureReason.Unavailable);
        
        HandleResult<ShipmentResult>(x => !x.IsShipmentAccepted);

    }
}