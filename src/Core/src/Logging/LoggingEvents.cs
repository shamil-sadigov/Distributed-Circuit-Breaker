namespace Core.Logging;

public static class LoggingEvents
{
    public static class NewCircuitBreakerContextCreated
    {
        public const int EventId = 100;
        public const string EventName = "NewCircuitBreakerContextCreated";
    }
    
    public static class CircuitBreakerStateReport
    {
        public const int EventId = 101;
        public const string EventName = "CircuitBreakerStateReport";
    }
}