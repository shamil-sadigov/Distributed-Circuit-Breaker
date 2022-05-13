using DCB.Core.Handlers.ResultHandlers;
using DCB.Core.Tests.ResultHandler.Helpers;
using FluentAssertions;

namespace DCB.Core.Tests.ResultHandler;

public class ResultHandlerTests
{
    [Fact]
    public void Can_handle_specified_result()
    {
        var resultHandlers = new ResultHandlers();
        resultHandlers.Handle<CustomResult>(x => !x.IsSuccessful);
        var customResult = new CustomResult(IsSuccessful: false);

        resultHandlers.CanHandle(customResult)
            .Should().BeTrue();
    }
    
    [Fact]
    public void Can_handle_specified_result_with_customer_handler()
    {
        var resultHandlers = new ResultHandlers();
        resultHandlers.Handle(new CustomResultHandler());
        var customResult = new CustomResult(IsSuccessful: false);

        resultHandlers.CanHandle(customResult)
            .Should().BeTrue();
    }
    
    [Fact]
    public void Can_handle_multiple_specified_results_of_different_type()
    {
        var resultHandlers = new ResultHandlers();
        
        resultHandlers
            .Handle<CustomResult>(x => !x.IsSuccessful)
            .Handle<DbResult>(x => !x.IsDataSaved);
        
        var customResult = new CustomResult(IsSuccessful: false);
        var dbResult = new DbResult(IsDataSaved: false);

        resultHandlers.CanHandle(customResult)
            .Should().BeTrue();
        
        resultHandlers.CanHandle(dbResult)
            .Should().BeTrue();
    }
    
    [Fact]
    public void Cannot_handle_unspecified_result()
    {
        var resultHandlers = new ResultHandlers();
        
        resultHandlers.Handle<CustomResult>(x => !x.IsSuccessful);
        
        var customResult = new CustomResult(IsSuccessful: true);
        var dbResult = new DbResult(IsDataSaved: false);

        resultHandlers.CanHandle(customResult)
            .Should().BeFalse();

        resultHandlers.CanHandle(dbResult)
            .Should().BeFalse();
    }
}