using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;

using Summaries.ArchitectureTests.Common;

namespace Summaries.ArchitectureTests.NamingRules;

public sealed class ControllerNamingTests
{
    [Fact]
    public void Concrete_Controllers_Should_End_With_Controller()
    {
        var result = Types
            .InAssembly(Assemblies.Api)
            .That()
            .Inherit(typeof(ControllerBase))
            .And()
            .AreNotAbstract()
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "All concrete API controllers must end with 'Controller'.");
    }
}