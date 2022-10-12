namespace IntegrationTests.WebAppConfiguration.Shipment.ShipmentStrategies;

public class RateLimitedStrategy : IShipmentStrategy
{
    public Task<ShipmentResult> SaveLogAsync(object metrics)
    {
        throw new ShipmentServiceConnectionException(FailureReason.RateLimited);
    }
}