using DCB.Core.Handlers.ExceptionHandlers;
using DCB.Core.Handlers.ResultHandlers;

namespace DCB.Core.Tests.HandlerTests;

public class CustomResultHandler:IResultHandler<CustomResult>
{
    public bool HandleResult(CustomResult result) => !result.IsSuccessful;
}
