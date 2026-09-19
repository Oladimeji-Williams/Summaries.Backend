using NetArchTest.Rules;

using Summaries.ArchitectureTests.Common;

namespace Summaries.ArchitectureTests.LayerRules;

/// <summary>
/// Each feature module (Authentication, Books, Payments, Users, Admin) is
/// its own class library, and none of them may reference any other.
/// Anything one module needs to expose for another to consume (e.g. Users'
/// UserProfileDto/UserErrors, Books' IBookCatalog, Payments' IPurchaseVerifier)
/// lives in Summaries.SharedKernel.Contracts instead — a shared, neutral
/// location every module can depend on without depending on each other.
/// These tests fail the build if that ever regresses.
/// </summary>
public sealed class ModuleIsolationTests
{
    [Fact]
    public void Modules_Should_Not_Depend_On_Each_Other()
    {
        var allNames = Assemblies.Modules.Select(m => m.Name).ToArray();
        var failures = new List<string>();

        foreach (var (name, assembly) in Assemblies.Modules)
        {
            foreach (var otherName in allNames.Where(n => n != name))
            {
                var result = Types
                    .InAssembly(assembly)
                    .ShouldNot()
                    .HaveDependencyOn($"Summaries.Modules.{otherName}")
                    .GetResult();

                if (!result.IsSuccessful)
                {
                    failures.Add($"{name} module must not depend on {otherName} module.");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void Modules_Should_Not_Depend_On_Api()
    {
        var failures = new List<string>();

        foreach (var (name, assembly) in Assemblies.Modules)
        {
            var result = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOn("Summaries.API")
                .GetResult();

            if (!result.IsSuccessful)
            {
                failures.Add($"{name} module must not depend on the API host project.");
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }
}
