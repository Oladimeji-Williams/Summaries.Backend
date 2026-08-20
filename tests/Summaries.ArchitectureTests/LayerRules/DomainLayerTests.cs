using NetArchTest.Rules;

using Summaries.ArchitectureTests.Common;

namespace Summaries.ArchitectureTests.LayerRules;

public sealed class DomainLayerTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Application()
    {
        var result = Types
            .InAssembly(Assemblies.Domain)
            .ShouldNot()
            .HaveDependencyOn("Summaries.Application")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Domain must not depend on Application.");
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Persistence()
    {
        var result = Types
            .InAssembly(Assemblies.Domain)
            .ShouldNot()
            .HaveDependencyOn("Summaries.Persistence")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Domain must not depend on Persistence.");
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types
            .InAssembly(Assemblies.Domain)
            .ShouldNot()
            .HaveDependencyOn("Summaries.Infrastructure")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Domain must not depend on Infrastructure.");
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Api()
    {
        var result = Types
            .InAssembly(Assemblies.Domain)
            .ShouldNot()
            .HaveDependencyOn("Summaries.API")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Domain must not depend on API.");
    }
}