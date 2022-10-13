namespace FunctionalTests.WebAppConfiguration.Shipment.ShipmentStrategies;

public class RateLimitedStrategy : IShipmentStrategy
{
    public Task<ShipmentResult> SaveLogAsync(object metrics)
    {
        throw new ShipmentServiceException(FailureReason.RateLimited);
    }
}