using DCB.Core.Handlers.ResultHandlers;

namespace DCB.Core.Tests.ResultHandlerTests.Helpers;

public class CustomResultHandler : IResultHandler<CustomResult>
{
    public bool HandleResult(CustomResult result)
    {
        return !result.IsSuccessful;
    }
}