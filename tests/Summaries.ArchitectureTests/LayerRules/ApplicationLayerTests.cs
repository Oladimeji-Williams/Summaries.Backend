using NetArchTest.Rules;

using Summaries.ArchitectureTests.Common;

namespace Summaries.ArchitectureTests.LayerRules;

public sealed class ApplicationLayerTests
{
    [Fact]
    public void Application_Should_Not_Depend_On_Persistence()
    {
        var result = Types
            .InAssembly(Assemblies.Application)
            .ShouldNot()
            .HaveDependencyOn("Summaries.Persistence")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Application must not depend on Persistence.");
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types
            .InAssembly(Assemblies.Application)
            .ShouldNot()
            .HaveDependencyOn("Summaries.Infrastructure")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Application must not depend on Infrastructure.");
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Api()
    {
        var result = Types
            .InAssembly(Assemblies.Application)
            .ShouldNot()
            .HaveDependencyOn("Summaries.API")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Application must not depend on API.");
    }
}