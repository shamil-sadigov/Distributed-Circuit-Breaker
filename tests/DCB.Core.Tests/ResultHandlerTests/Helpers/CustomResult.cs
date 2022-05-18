namespace DCB.Core.Tests.ResultHandler.Helpers;

public record CustomResult(bool IsSuccessful);

public record DbResult(bool IsDataSaved);