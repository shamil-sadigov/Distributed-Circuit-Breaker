using System.Net;
using DCB.Client.WebApi.CircuitBreakerOptions;
using DCB.Client.WebApi.Dto;
using DCB.Core.CircuitBreakers;
using Microsoft.AspNetCore.Mvc;

namespace DCB.Client.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly EventStore _eventStore;
    private readonly ICircuitBreaker<EventStoreCircuitBreakerOptions> _circuitBreaker;

    public EventController(EventStore eventStore, ICircuitBreaker<EventStoreCircuitBreakerOptions> circuitBreaker)
    {
        _eventStore = eventStore;
        _circuitBreaker = circuitBreaker;
    }

    [HttpPost]
    public async Task<ActionResult<SendEventResponse>> Post(SendEventRequest request, CancellationToken token)
    {
        if (await _circuitBreaker.IsOpenAsync())
            return StatusCode((int) HttpStatusCode.ServiceUnavailable);

        try
        {
            var result = await _circuitBreaker.ExecuteAsync(async _ => 
                await _eventStore.SaveEventAsync(request.EventMessage), token);

            if (result.IsEventSent)
                return Ok(SendEventResponse.Successful);
            
            return SendEventResponse.Failed(EventStoreFailureReason.Unknown);

        }
        catch (EventStoreConnectionException exception)
        {
            return SendEventResponse.Failed(exception.FailureReason);
        }
    }
}