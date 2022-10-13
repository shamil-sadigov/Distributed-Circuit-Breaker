namespace FunctionalTests.WebAppConfiguration.Shipment.ShipmentStrategies;

public class ClientUnauthorizedStrategy : IShipmentStrategy
{
    public Task<ShipmentResult> SaveLogAsync(object metrics)
    {
        throw new ShipmentServiceException(FailureReason.Unauthorized);
    }
}