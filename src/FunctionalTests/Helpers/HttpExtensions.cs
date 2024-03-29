﻿using System.Net;
using FluentAssertions;

namespace FunctionalTests.Helpers;

public static class HttpExtensions
{
    public static void ShouldBe(this HttpResponseMessage httpResponse, HttpStatusCode statusCode, string because = "")
    {
        httpResponse.StatusCode.Should().Be(statusCode, because);
    }
}