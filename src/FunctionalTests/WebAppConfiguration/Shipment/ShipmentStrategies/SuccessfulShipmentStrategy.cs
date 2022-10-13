namespace FunctionalTests.WebAppConfiguration.Shipment.ShipmentStrategies;

public class SuccessfulShipmentStrategy:IShipmentStrategy
{
    public Task<ShipmentResult> SaveLogAsync(object metrics)
    {
        return Task.FromResult(new ShipmentResult()
        {
            IsShipmentAccepted = true
        });
    }
}