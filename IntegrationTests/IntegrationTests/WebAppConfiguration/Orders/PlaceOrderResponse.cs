namespace IntegrationTests.WebAppConfiguration.Orders;

public class PlaceOrderResponse
{
    public bool Succeeded { get; set; }

    public static readonly PlaceOrderResponse Successful = new()
    {
        Succeeded = true
    };
    
    public static PlaceOrderResponse Failed => new()
    {
        Succeeded = false
    };
}

