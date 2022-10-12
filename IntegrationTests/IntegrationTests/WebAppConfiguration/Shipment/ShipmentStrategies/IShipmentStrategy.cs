namespace IntegrationTests.WebAppConfiguration.Prometheus.ShipmentStrategies;

public interface IShipmentStrategy
{
    Task<ShipmentResult> SaveLogAsync(object metrics);
}