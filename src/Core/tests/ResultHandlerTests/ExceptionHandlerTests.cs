using System.Data;
using System.Net;
using Core.Settings.Handlers.ExceptionHandlers;
using Core.Tests.ResultHandlerTests.Helpers;

namespace Core.Tests.ResultHandlerTests;

public class ExceptionHandlerTests
{
    [Fact]
    public void Can_handle_specified_exception()
    {
        var exceptionHandlers = new ExceptionHandlers();

        exceptionHandlers.Handle<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.TooManyRequests);

        var httpException = new CustomHttpException(HttpStatusCode.TooManyRequests);

        exceptionHandlers.CanHandle(httpException)
            .Should().BeTrue();
    }

    // TODO: Make IExceptionHandler generic

    [Fact]
    public void Can_handle_multiple_specified_exceptions_of_different_type()
    {
        var exceptionHandlers = new ExceptionHandlers();

        exceptionHandlers
            .Handle<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable)
            .Handle<DBConcurrencyException>();

        var httpException = new CustomHttpException(HttpStatusCode.ServiceUnavailable);
        var dbConcurrencyException = new DBConcurrencyException();

        exceptionHandlers.CanHandle(httpException)
            .Should().BeTrue();

        exceptionHandlers.CanHandle(dbConcurrencyException)
            .Should().BeTrue();
    }

    [Fact]
    public void Cannot_handle_unspecified_exception()
    {
        var exceptionHandlers = new ExceptionHandlers();

        exceptionHandlers.Handle<DBConcurrencyException>();

        var httpException = new CustomHttpException(HttpStatusCode.ServiceUnavailable);
        var dbConcurrencyException = new DBConcurrencyException();

        exceptionHandlers.CanHandle(httpException)
            .Should().BeFalse();

        exceptionHandlers.CanHandle(dbConcurrencyException)
            .Should().BeTrue();
    }
}