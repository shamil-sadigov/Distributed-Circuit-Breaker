using System.Net;
using System.Net.Http.Json;
using DCB.Client.CriticalLogSaver.Dto;
using DCB.Client.TraceLogSaver.Dto;
using FluentAssertions;

namespace DCB.IntegrationTests.Helpers;

public static class HttpExtensions
{
    public static void ShouldBe(this HttpResponseMessage httpResponse, HttpStatusCode statusCode, string because = "")
    {
        httpResponse.StatusCode.Should().Be(statusCode, because);
    }
    
    public static void ShouldAllBe(
        this HttpResponseMessage[] httpResponses,
        HttpStatusCode statusCode,
        string because = "")
    {
        httpResponses.Should().AllSatisfy(x => x.ShouldBe(statusCode, because));
    }
    
    public static async Task<TResponse?> SendLogRequestAsync<TResponse>(
        this HttpClient httpClient,
        SaveTraceLogRequest request)
    {
        var httpResponse = await httpClient.SendLogRequestAsync(request);
        return await httpResponse.ToJsonAsync<TResponse>();
    }
    
    public static async Task<HttpResponseMessage> SendLogRequestAsync(
        this HttpClient httpClient,
        SaveTraceLogRequest request)
    {
        return await httpClient.PostAsJsonAsync(LogSaverUris.SaveTraceLogPath, request);
    }
    
    public static async Task<TResponse?> SendLogRequestAsync<TResponse>(
        this HttpClient httpClient,
        SaveCriticalLogRequest request)
    {
        var httpResponse = await httpClient.SendLogRequestAsync(request);
        return await httpResponse.ToJsonAsync<TResponse>();
    }
    
    public static async Task<HttpResponseMessage> SendLogRequestAsync(
        this HttpClient httpClient,
        SaveCriticalLogRequest request)
    {
        return await httpClient.PostAsJsonAsync(LogSaverUris.SaveCriticalLogPath, request);
    }
    
    public static async Task<TResult?> ToJsonAsync<TResult>(this HttpResponseMessage httpResponse)
    {
        return await httpResponse.Content.ReadFromJsonAsync<TResult>();
    }
}