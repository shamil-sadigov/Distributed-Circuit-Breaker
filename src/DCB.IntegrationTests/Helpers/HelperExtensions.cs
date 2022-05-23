using DCB.Client.CriticalLogSaver.Dto;
using DCB.Client.Shared;
using FluentAssertions;

namespace DCB.IntegrationTests.Helpers;

public static class HelperExtensions
{
    public static bool IsEvenNumber(this int number) => number % 2 == 0;
    
    public static bool IsOddNumber(this int number) => !number.IsEvenNumber();

    public static void ShouldNotBeSuccessfulBecause(
        this SavedLogResponse? savedLogResponse,
        LogStorageFailureReason failureReason)
    {
        savedLogResponse.Should().NotBeNull();
        savedLogResponse!.Succeeded.Should().BeFalse();
        savedLogResponse.FailureReason.Should().Be(failureReason);
    }
}