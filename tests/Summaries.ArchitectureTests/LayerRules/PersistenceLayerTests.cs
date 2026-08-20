using NetArchTest.Rules;

using Summaries.ArchitectureTests.Common;

namespace Summaries.ArchitectureTests.LayerRules;

public sealed class PersistenceLayerTests
{
    [Fact]
    public void Persistence_Should_Not_Depend_On_Api()
    {
        var result = Types
            .InAssembly(Assemblies.Persistence)
            .ShouldNot()
            .HaveDependencyOn("Summaries.API")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Persistence must not depend on API.");
    }

    [Fact]
    public void Persistence_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types
            .InAssembly(Assemblies.Persistence)
            .ShouldNot()
            .HaveDependencyOn("Summaries.Infrastructure")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "Persistence must not depend on Infrastructure.");
    }
}