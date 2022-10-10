using System.Net;
using DCB.Client.CriticalLogSaver.Dto;
using DCB.Client.Shared;
using DCB.Client.Shared.LogSavingStrategies;
using DCB.Client.TraceLogSaver.Dto;
using DCB.IntegrationTests.Helpers;
using FluentAssertions;
using Xunit.Abstractions;

namespace DCB.IntegrationTests;

public class LogSaverUris
{
    public const string SaveTraceLogPath = "tracelogs"; 
    public const string SaveCriticalLogPath = "criticallogs"; 
}

public class CircuitBreakerTests:IClassFixture<TraceLogSaverAppFactory>, IClassFixture<CriticalLogSaverAppFactory>
{
    
    private readonly TraceLogSaverAppFactory _traceLogSaverAppFactory;
    private readonly CriticalLogSaverAppFactory _criticalLogSaverAppFactory;
    private readonly ITestOutputHelper _testOutputHelper;

    public CircuitBreakerTests(
        TraceLogSaverAppFactory traceLogSaverAppFactory, 
        CriticalLogSaverAppFactory criticalLogSaverAppFactory, 
        ITestOutputHelper testOutputHelper)
    {
        _traceLogSaverAppFactory = traceLogSaverAppFactory;
        _criticalLogSaverAppFactory = criticalLogSaverAppFactory;
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public async Task Logs_are_successfully_saved_when_circuit_log_storage_is_available()
    {
        // Arrange
        HttpClient traceLogSaver = _traceLogSaverAppFactory.CreateClient();
        HttpClient criticalLogSaver = _criticalLogSaverAppFactory.CreateClient();

        var savedLogResponses = new List<SavedLogResponse>();

        // Act
        for (var counter = 0; counter < 50; counter++)
        {
            SavedLogResponse? response;

            if (counter.IsEvenNumber())
                response = await traceLogSaver.SendLogRequestAsync<SavedLogResponse>(CreateTraceLogRequest());
            else
                response = await criticalLogSaver.SendLogRequestAsync<SavedLogResponse>(CreateCriticalLogRequest());
            
            savedLogResponses.Add(response);
        }

        // Assert
        savedLogResponses.Should().AllSatisfy(x =>
        {
            x.Should().NotBeNull();
            x.Succeeded.Should().BeTrue();
        });
    }
    
    [Fact]
    public async Task When_log_storage_is_overwhelmed_then_circuitBreaker_is_opened_and_service_is_unavailable()
    {
        // Arrange
        HttpClient traceLogSaver = _traceLogSaverAppFactory.CreateClient();
        HttpClient criticalLogSaver = _criticalLogSaverAppFactory.CreateClient();
        
        var circuitBreakerOptions = _traceLogSaverAppFactory.GetService<LogStorageCircuitBreakerOptions>();
        
        var logStorage = _traceLogSaverAppFactory.GetService<LogStorage>();
        logStorage.SetSavingStrategy(new LogStorageIsOverwhelmedStrategy());
        
        // Act
        for (int counter = 0; counter < circuitBreakerOptions.FailureAllowedBeforeBreaking; counter++)
        {
            // Bring CircuitBreaker to Open State
            var saveLogResponse = await traceLogSaver.SendLogRequestAsync<SavedLogResponse>(CreateTraceLogRequest());
            saveLogResponse.ShouldNotBeSuccessfulBecause(LogStorageFailureReason.Overwhelmed);
        }

        HttpResponseMessage httpResponse = await criticalLogSaver.SendLogRequestAsync(CreateCriticalLogRequest());
        httpResponse.ShouldBe(HttpStatusCode.ServiceUnavailable, "Because CircuitBreaker is opened");
    }

    private static SaveTraceLogRequest CreateTraceLogRequest()
    {
        return new SaveTraceLogRequest()
        {
            LogMessage = $"[INFO]:{DateTime.UtcNow}:Very helpful info... "
        };
    }
    
    private static SaveCriticalLogRequest CreateCriticalLogRequest()
    {
        return new SaveCriticalLogRequest()
        {
            LogMessage = $"[INFO]:{DateTime.UtcNow}:Very helpful info... "
        };
    }
}