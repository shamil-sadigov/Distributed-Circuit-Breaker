#nullable disable
// For dto it's ok, because we use serializers for settings values

namespace Storage.Redis;

public class CircuitBreakerDataModel
{
    public string Name { get; set; }
    public int FailedTimes { get; set; }
    public DateTime? LastTimeFailed { get; set; }
}