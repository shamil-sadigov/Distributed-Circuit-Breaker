#nullable disable
// For dto it's ok, because we use serializers for setting values

namespace Storage.Redis;

public sealed record CircuitBreakerDataModel(string Name, int FailedTimes, DateTime? LastTimeFailed);