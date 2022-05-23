using System.Net;
using DCB.Client.WebApi.CircuitBreakerOptions;
using DCB.Client.WebApi.Dto;
using DCB.Core.CircuitBreakers;
using Microsoft.AspNetCore.Mvc;

namespace DCB.Client.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class EventsController : ControllerBase
{
    private readonly LogStorage _logStorage;
    private readonly ICircuitBreaker<EventStoreCircuitBreakerOptions> _circuitBreaker;

    public EventsController(LogStorage logStorage, ICircuitBreaker<EventStoreCircuitBreakerOptions> circuitBreaker)
    {
        _logStorage = logStorage;
        _circuitBreaker = circuitBreaker;
    }

    [HttpGet("Ping")]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }
    
    [HttpPost]
    public async Task<ActionResult<SendEventResponse>> Post(SendEventRequest request, CancellationToken token)
    {
        if (await _circuitBreaker.IsOpenAsync())
            return StatusCode((int) HttpStatusCode.ServiceUnavailable);

        try
        {
            var result = await _circuitBreaker.ExecuteAsync(async _ => 
                await _logStorage.SaveLogAsync(request.EventMessage), token);

            if (result.IsLogSaved)
                return Ok(SendEventResponse.Successful);
            
            return SendEventResponse.Failed(EventStoreFailureReason.Unknown);

        }
        catch (EventStoreConnectionException exception)
        {
            return SendEventResponse.Failed(exception.FailureReason);
        }
    }
}