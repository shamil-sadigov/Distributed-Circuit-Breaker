namespace Core.Settings.Handlers.ResultHandlers;

public interface IResultHandler<in TResult>
{
    bool HandleResult(TResult result);
}