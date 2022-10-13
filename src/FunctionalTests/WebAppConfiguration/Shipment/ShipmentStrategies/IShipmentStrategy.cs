namespace FunctionalTests.WebAppConfiguration.Shipment.ShipmentStrategies;

public interface IShipmentStrategy
{
    Task<ShipmentResult> SaveLogAsync(object metrics);
}