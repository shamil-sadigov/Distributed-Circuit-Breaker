using Core.Handlers.ResultHandlers;

namespace Core.Tests.ResultHandlerTests.Helpers;

public class CustomResultHandler : IResultHandler<CustomResult>
{
    public bool HandleResult(CustomResult result)
    {
        return !result.IsSuccessful;
    }
}