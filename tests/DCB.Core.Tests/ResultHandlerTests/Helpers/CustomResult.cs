namespace DCB.Core.Tests.ResultHandlerTests.Helpers;

public record CustomResult(bool IsSuccessful);

public record DbResult(bool IsDataSaved);