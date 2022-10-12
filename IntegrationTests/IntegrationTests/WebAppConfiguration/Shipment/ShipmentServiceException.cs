namespace IntegrationTests.WebAppConfiguration.Shipment;

public class ShipmentServiceException:Exception
{
    public FailureReason FailureReason { get; }

    public ShipmentServiceException(FailureReason failureReason)
    {
        FailureReason =  failureReason;
    }
}