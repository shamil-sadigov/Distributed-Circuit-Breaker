namespace FunctionalTests.WebAppConfiguration.Shipment.ShipmentStrategies;

public class UnsuccessfulSavingStrategy:IShipmentStrategy
{
    public Task<ShipmentResult> SaveLogAsync(object metrics)
    {
        return Task.FromResult(new ShipmentResult()
        {
            IsShipmentAccepted = false
        });
    }
}