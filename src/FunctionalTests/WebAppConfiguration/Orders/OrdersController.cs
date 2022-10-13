using Core;
using Core.Context;
using FunctionalTests.WebAppConfiguration.Shipment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FunctionalTests.WebAppConfiguration.Orders;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ShipmentService _shipmentService;
    private readonly ICircuitBreaker<ShipmentServicePolicy> _circuitBreaker;

    public OrdersController(
        ShipmentService shipmentService, 
        ICircuitBreaker<ShipmentServicePolicy> circuitBreaker)
    {
        _shipmentService = shipmentService;
        _circuitBreaker = circuitBreaker;
    }
    
    [HttpPost]
    public async Task<ActionResult<PlaceOrderResponse>> Post(PlaceOrderRequest orderRequest, CancellationToken token)
    {
        if (await _circuitBreaker.GetStateAsync(token) == CircuitBreakerState.Open)
        {
            // Hmm, seems like ShipmentService is not healthy, let's fail fast 
            return StatusCode(StatusCodes.Status503ServiceUnavailable, PlaceOrderResponse.Failed);
        }

        try
        {
            {
                // Place order, charge user, save in db, and other useful stuff...
            }
            
            // Well, now it's time to ship our order
            
            var shipmentResult = await _circuitBreaker.ExecuteAsync(async _ => 
                await _shipmentService.ShipOrderAsync(orderRequest.OrderId), token);

            if (shipmentResult.IsShipmentAccepted)
                return PlaceOrderResponse.Successful;
            
            return StatusCode(StatusCodes.Status409Conflict, PlaceOrderResponse.Failed);

        }
        catch (ShipmentServiceException)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, PlaceOrderResponse.Failed);
        }
    }
}