namespace DCB.Core.Storage;

// TODO: Register in IoC
public interface ICircuitBreakerStorage :
    ICircuitBreakerContextGetter, 
    ICircuitBreakerContextUpdater, 
    ICircuitBreakerContextAdder
{

}