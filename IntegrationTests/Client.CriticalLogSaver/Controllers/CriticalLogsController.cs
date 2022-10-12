using System.Net;
using Client.CriticalLogSaver.Dto;
using Client.Shared;
using Client.Shared.LogSavingStrategies;
using Core;
using Core.Context;
using Microsoft.AspNetCore.Mvc;

namespace Client.CriticalLogSaver.Controllers;

[ApiController]
[Route("[controller]")]
public class CriticalLogsController : ControllerBase
{
    private readonly LogStorage _logStorage;
    private readonly ICircuitBreaker<LogStorageCircuitBreakerOptions> _circuitBreaker;

    public CriticalLogsController(LogStorage logStorage, ICircuitBreaker<LogStorageCircuitBreakerOptions> circuitBreaker)
    {
        _logStorage = logStorage;
        _circuitBreaker = circuitBreaker;
    }

    [HttpGet("ping")]
    public IActionResult Ping() => Ok("Pong");

    [HttpPost]
    public async Task<ActionResult<SavedLogResponse>> Post(SaveCriticalLogRequest request, CancellationToken token)
    {
        if (await _circuitBreaker.GetStateAsync(token) == CircuitBreakerState.Open)
            return StatusCode((int) HttpStatusCode.ServiceUnavailable);

        try
        {
            var result = await _circuitBreaker.ExecuteAsync(async _ => 
                await _logStorage.SaveLogAsync(request.LogMessage), token);

            if (result.IsLogSaved)
                return Ok(SavedLogResponse.Successful);
            
            return SavedLogResponse.Failed(PrometheusFailureReason.Unknown);

        }
        catch (PrometheusConnectionException exception)
        {
            return SavedLogResponse.Failed(exception.FailureReason);
        }
    }
}