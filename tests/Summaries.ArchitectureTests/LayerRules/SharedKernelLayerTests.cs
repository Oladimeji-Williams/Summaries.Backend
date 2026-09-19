using NetArchTest.Rules;

using Summaries.ArchitectureTests.Common;

namespace Summaries.ArchitectureTests.LayerRules;

/// <summary>
/// SharedKernel is the innermost, most stable layer: every module and
/// Shared.Infrastructure may depend on it, but it must never depend
/// outward on a module, on Shared.Infrastructure, or on API.
/// </summary>
public sealed class SharedKernelLayerTests
{
    [Fact]
    public void SharedKernel_Should_Not_Depend_On_Any_Module()
    {
        var result = Types
            .InAssembly(Assemblies.SharedKernel)
            .ShouldNot()
            .HaveDependencyOn("Summaries.Modules")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "SharedKernel must not depend on any feature module.");
    }

    [Fact]
    public void SharedKernel_Should_Not_Depend_On_SharedInfrastructure_Or_Api()
    {
        var result = Types
            .InAssembly(Assemblies.SharedKernel)
            .ShouldNot()
            .HaveDependencyOnAny("Summaries.Shared.Infrastructure", "Summaries.API")
            .GetResult();

        Assert.True(
            result.IsSuccessful,
            "SharedKernel must not depend on Shared.Infrastructure or API.");
    }
}
