﻿#nullable disable
// For dto it's ok, because we use serializers for settings values

namespace Storage.Redis;

public sealed record CircuitBreakerDataModel(string Name, int FailedTimes, DateTime? LastTimeFailed);