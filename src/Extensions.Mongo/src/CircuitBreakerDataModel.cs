using MongoDB.Bson.Serialization.Attributes;

namespace Registration.Mongo;

public class CircuitBreakerDataModel
{
    [BsonId] 
    public string Name { get; set; }

    public int FailureAllowedBeforeBreaking { get; set; }
    public int FailedCount { get; set; }
    
    public bool IsCircuitBreakerClosed { get; set; }
    public DateTime? TransitionDateToHalfOpenState { get; set; }
    public DateTime? LastTimeStateChanged { get; set; }
    public TimeSpan DurationOfBreak { get; set; }
}