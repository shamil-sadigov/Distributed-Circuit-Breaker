namespace DCB.Client.Shared.LogSavingStrategies;

public interface ILogSavingStrategy
{
    Task<SavedLogResult> SaveLogAsync(string logMessage);
}