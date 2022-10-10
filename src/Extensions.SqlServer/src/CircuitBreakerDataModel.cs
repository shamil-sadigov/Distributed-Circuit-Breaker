namespace Registration.SqlServer;

public class CircuitBreakerDataModel
{
    public string Name { get; set; }
    public int FailedCount { get; set; }
    public DateTime? LastTimeStateChanged { get; set; }
    public TimeSpan DurationOfBreak { get; set; }
}