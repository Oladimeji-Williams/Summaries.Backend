using NetArchTest.Rules;

using Summaries.ArchitectureTests.Common;

namespace Summaries.ArchitectureTests.LayerRules;

public sealed class InfrastructureLayerTests
{
    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Persistence()
    {
        var result = Types
            .InAssembly(Assemblies.Infrastructure)
            .ShouldNot()
            .HaveDependencyOn("Summaries.Persistence")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Infrastructure must not depend on Persistence.");
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var result = Types
            .InAssembly(Assemblies.Infrastructure)
            .ShouldNot()
            .HaveDependencyOn("Summaries.API")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Infrastructure must not depend on API.");
    }
}