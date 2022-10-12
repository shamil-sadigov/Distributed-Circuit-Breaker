namespace IntegrationTests.WebAppConfiguration.Shipment;

public class ShipmentServiceConnectionException:Exception
{
    public FailureReason FailureReason { get; }

    public ShipmentServiceConnectionException(FailureReason failureReason)
    {
        FailureReason =  failureReason;
    }
}