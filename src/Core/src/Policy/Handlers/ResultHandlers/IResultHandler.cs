namespace Core.Policy.Handlers.ResultHandlers;

public interface IResultHandler<in TResult>
{
    bool HandleResult(TResult result);
}