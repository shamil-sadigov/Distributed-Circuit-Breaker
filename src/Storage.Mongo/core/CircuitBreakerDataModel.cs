using MongoDB.Bson.Serialization.Attributes;

namespace Storage.Mongo;

public sealed class CircuitBreakerDataModel
{
    [BsonId] 
    public string Name { get; set; }
    
    public int FailedTimes { get; set; }
    
    public DateTime? LastTimeFailed { get; set; }
}