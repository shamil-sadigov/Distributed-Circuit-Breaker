using IntegrationTests.WebAppConfiguration.Shipment.ShipmentStrategies;

namespace IntegrationTests.WebAppConfiguration.Shipment;

/// <summary>
/// Mock service for saving metrics into Prometheus.
/// This class is for testing of CircuitBreaker purposes.
/// </summary>
public class ShipmentService
{
    public IShipmentStrategy? ShipmentStrategy { get; set; }
    
    /// <summary>
    /// Sends log to remote service
    /// </summary>
    public Task<ShipmentResult> ShipOrderAsync(object order)
    {
        if (ShipmentStrategy is null)
        {
            throw new InvalidOperationException("Set ShipmentStrategy before shipping order!");
        }
        
        return ShipmentStrategy.SaveLogAsync(order);
    }
}