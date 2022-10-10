namespace Core.Storage;

public interface ICircuitBreakerStorage :
    ICircuitBreakerContextGetter,
    ICircuitBreakerContextUpdater,
    ICircuitBreakerContextAdder
{
}