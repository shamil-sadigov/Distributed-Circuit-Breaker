using DCB.Client.WebApi.Dto;

namespace DCB.Client.WebApi.EventSavingStrategies;

public interface ILogSavingStrategy
{
    Task<SavedLogResult> SaveLogAsync(string logMessage);
}