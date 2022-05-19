using System.Net;

namespace DCB.Core.Tests.ResultHandlerTests.Helpers;

public class CustomHttpException : Exception
{
    public CustomHttpException(HttpStatusCode httpStatus)
    {
        HttpStatus = httpStatus;
    }

    public HttpStatusCode HttpStatus { get; }
}