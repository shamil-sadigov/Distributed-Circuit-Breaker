using DCB.Core.Handlers.ResultHandlers;

namespace DCB.Core.Tests.ResultHandler.Helpers;

public class CustomResultHandler:IResultHandler<CustomResult>
{
    public bool HandleResult(CustomResult result) => !result.IsSuccessful;
}
