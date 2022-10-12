using Core.Context;
using Microsoft.Extensions.Logging;

namespace Core.Logging;

/// <summary>
/// Logging extensions that promotes high-performance logging
/// </summary>
public static partial class LoggingExtensions
{
    internal static void LogCircuitBreakerContextCreated(this ILogger logger, CircuitBreakerContext context)
    {
        NewCircuitBreakerContextCreated(logger,context.Name);
    }
    
    internal static void LogCircuitBreakerState(this ILogger logger, CircuitBreakerLoggingContext context)
    {
        CircuitBreakerState(logger, context.CircuitBreakerName, context.CircuitBreakerState, context);
    }
    
    [LoggerMessage
    (
        EventId = LoggingEvents.NewCircuitBreakerContextCreated.EventId, 
        EventName = LoggingEvents.NewCircuitBreakerContextCreated.EventName, 
        Level = LogLevel.Debug, 
        Message = "Created new CircuitBreaker context '{CircuitBreakerName}'"
    )]
    static partial void NewCircuitBreakerContextCreated(
        ILogger logger,
        string circuitBreakerName);
    
    [LoggerMessage
    (
        EventId = LoggingEvents.CircuitBreakerStateReport.EventId, 
        EventName = LoggingEvents.CircuitBreakerStateReport.EventName, 
        Level = LogLevel.Debug, 
        Message = "CircuitBreaker '{CircuitBreakerName}' is in '{CircuitBreakerState}' state. Context: {Context}."
    )]
    static partial void CircuitBreakerState(
        ILogger logger,
        string circuitBreakerName, 
        CircuitBreakerState circuitBreakerState,
        CircuitBreakerLoggingContext context);
}