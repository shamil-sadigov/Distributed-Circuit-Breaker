using DCB.Core.Handlers.ResultHandlers;
using DCB.Core.Tests.ResultHandlerTests.Helpers;
using FluentAssertions;

namespace DCB.Core.Tests.ResultHandlerTests;

public class ResultHandlerTests
{
    [Fact]
    public void Can_handle_specified_result()
    {
        var resultHandlers = new ResultHandlers();
        resultHandlers.Handle<CustomResult>(x => !x.IsSuccessful);
        var customResult = new CustomResult(false);

        resultHandlers.CanHandle(customResult)
            .Should().BeTrue();
    }

    [Fact]
    public void Can_handle_specified_result_with_customer_handler()
    {
        var resultHandlers = new ResultHandlers();
        resultHandlers.Handle(new CustomResultHandler());
        var customResult = new CustomResult(false);

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

        var customResult = new CustomResult(false);
        var dbResult = new DbResult(false);

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

        var customResult = new CustomResult(true);
        var dbResult = new DbResult(false);

        resultHandlers.CanHandle(customResult)
            .Should().BeFalse();

        resultHandlers.CanHandle(dbResult)
            .Should().BeFalse();
    }
}