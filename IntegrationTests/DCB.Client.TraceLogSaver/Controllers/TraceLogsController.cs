using System.Net;
using DCB.Client.Shared;
using DCB.Client.Shared.LogSavingStrategies;
using DCB.Client.TraceLogSaver.Dto;
using DCB.Core.CircuitBreakers;
using Microsoft.AspNetCore.Mvc;

namespace DCB.Client.TraceLogSaver.Controllers;

[ApiController]
[Route("[controller]")]
public class TraceLogsController : ControllerBase
{
    private readonly LogStorage _logStorage;
    private readonly ICircuitBreaker<LogStorageCircuitBreakerOptions> _circuitBreaker;

    public TraceLogsController(LogStorage logStorage, ICircuitBreaker<LogStorageCircuitBreakerOptions> circuitBreaker)
    {
        _logStorage = logStorage;
        _circuitBreaker = circuitBreaker;
    }

    [HttpGet("ping")]
    public IActionResult Ping() => Ok("Pong");
    
    [HttpPost]
    public async Task<ActionResult<SavedLogResponse>> Post(SaveTraceLogRequest request, CancellationToken token)
    {
        if (await _circuitBreaker.IsOpenAsync(token))
            return StatusCode((int) HttpStatusCode.ServiceUnavailable);

        try
        {
            var result = await _circuitBreaker.ExecuteAsync(async _ => 
                await _logStorage.SaveLogAsync(request.LogMessage), token);

            if (result.IsLogSaved)
                return Ok(SavedLogResponse.Successful);
            
            return SavedLogResponse.Failed(LogStorageFailureReason.Unknown);

        }
        catch (EventStoreConnectionException exception)
        {
            return SavedLogResponse.Failed(exception.FailureReason);
        }
    }
}