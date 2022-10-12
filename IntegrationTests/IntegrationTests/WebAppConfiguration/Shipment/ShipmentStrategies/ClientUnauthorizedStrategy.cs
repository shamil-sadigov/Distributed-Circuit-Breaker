namespace IntegrationTests.WebAppConfiguration.Prometheus.ShipmentStrategies;

public class ClientUnauthorizedStrategy : IShipmentStrategy
{
    public Task<ShipmentResult> SaveLogAsync(object metrics)
    {
        throw new ShipmentServiceConnectionException(FailureReason.Unauthorized);
    }
}