namespace FunctionalTests.WebAppConfiguration.Shipment.ShipmentStrategies;

public class ServiceUnavailableStrategy : IShipmentStrategy
{
    public Task<ShipmentResult> SaveLogAsync(object metrics)
    {
        throw new ShipmentServiceException(FailureReason.Unavailable);
    }
}