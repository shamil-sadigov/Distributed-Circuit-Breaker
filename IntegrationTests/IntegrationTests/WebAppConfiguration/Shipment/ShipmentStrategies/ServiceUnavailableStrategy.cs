namespace IntegrationTests.WebAppConfiguration.Prometheus.ShipmentStrategies;

public class ServiceUnavailableStrategy : IShipmentStrategy
{
    public Task<ShipmentResult> SaveLogAsync(object metrics)
    {
        throw new ShipmentServiceConnectionException(FailureReason.Unavailable);
    }
}