namespace DCB.Core.Handlers.ResultHandlers;


public interface IResultHandler<in TResult>
{
    bool HandleResult(TResult result);
}