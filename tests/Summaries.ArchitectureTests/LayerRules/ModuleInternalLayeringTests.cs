using NetArchTest.Rules;

using Summaries.ArchitectureTests.Common;

namespace Summaries.ArchitectureTests.LayerRules;

/// <summary>
/// Each module is a single class library containing its own Api/Application/
/// Domain/Infrastructure/Persistence folders — there's no compiler-enforced
/// project boundary between them the way there is between modules. These
/// tests are what actually enforces the inward-pointing layering
/// (Domain &lt; Application &lt; Infrastructure/Persistence &lt; Api) within
/// each module, using namespace conventions instead of assembly boundaries.
/// A module only gets a rule for a layer it actually has — Users and Admin,
/// for example, have no Domain/Infrastructure/Persistence of their own.
/// </summary>
public sealed class ModuleInternalLayeringTests
{
    private sealed record ModuleLayers(string Name, bool HasDomain, bool HasInfrastructure, bool HasPersistence);

    private static readonly ModuleLayers[] Layouts =
    [
        new("Authentication", HasDomain: false, HasInfrastructure: true, HasPersistence: true),
        new("Books", HasDomain: true, HasInfrastructure: false, HasPersistence: true),
        new("Payments", HasDomain: true, HasInfrastructure: true, HasPersistence: true),
        new("Users", HasDomain: false, HasInfrastructure: false, HasPersistence: false),
        new("Admin", HasDomain: false, HasInfrastructure: false, HasPersistence: false),
    ];

    [Fact]
    public void Domain_Should_Not_Depend_On_Outer_Layers_Within_Its_Own_Module()
    {
        var failures = new List<string>();

        foreach (var layout in Layouts.Where(l => l.HasDomain))
        {
            var assembly = Assemblies.Modules.Single(m => m.Name == layout.Name).Assembly;
            var outer = new[] { "Application", "Infrastructure", "Persistence", "Api" }
                .Select(layer => $"Summaries.Modules.{layout.Name}.{layer}")
                .ToArray();

            var result = Types
                .InAssembly(assembly)
                .That()
                .ResideInNamespace($"Summaries.Modules.{layout.Name}.Domain")
                .ShouldNot()
                .HaveDependencyOnAny(outer)
                .GetResult();

            if (!result.IsSuccessful)
            {
                failures.Add($"{layout.Name}.Domain must not depend on its own Application/Infrastructure/Persistence/Api.");
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void Application_Should_Not_Depend_On_Infrastructure_Persistence_Or_Api_Within_Its_Own_Module()
    {
        var failures = new List<string>();

        foreach (var layout in Layouts)
        {
            var assembly = Assemblies.Modules.Single(m => m.Name == layout.Name).Assembly;
            var outer = new List<string> { $"Summaries.Modules.{layout.Name}.Api" };
            if (layout.HasInfrastructure)
            {
                outer.Add($"Summaries.Modules.{layout.Name}.Infrastructure");
            }
            if (layout.HasPersistence)
            {
                outer.Add($"Summaries.Modules.{layout.Name}.Persistence");
            }

            var result = Types
                .InAssembly(assembly)
                .That()
                .ResideInNamespace($"Summaries.Modules.{layout.Name}.Application")
                .ShouldNot()
                .HaveDependencyOnAny([.. outer])
                .GetResult();

            if (!result.IsSuccessful)
            {
                failures.Add($"{layout.Name}.Application must not depend on its own Infrastructure/Persistence/Api.");
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void Infrastructure_And_Persistence_Should_Not_Depend_On_Api_Within_Its_Own_Module()
    {
        var failures = new List<string>();

        foreach (var layout in Layouts.Where(l => l.HasInfrastructure || l.HasPersistence))
        {
            var assembly = Assemblies.Modules.Single(m => m.Name == layout.Name).Assembly;

            if (layout.HasInfrastructure)
            {
                var result = Types
                    .InAssembly(assembly)
                    .That()
                    .ResideInNamespace($"Summaries.Modules.{layout.Name}.Infrastructure")
                    .ShouldNot()
                    .HaveDependencyOn($"Summaries.Modules.{layout.Name}.Api")
                    .GetResult();

                if (!result.IsSuccessful)
                {
                    failures.Add($"{layout.Name}.Infrastructure must not depend on its own Api.");
                }
            }

            if (layout.HasPersistence)
            {
                var result = Types
                    .InAssembly(assembly)
                    .That()
                    .ResideInNamespace($"Summaries.Modules.{layout.Name}.Persistence")
                    .ShouldNot()
                    .HaveDependencyOn($"Summaries.Modules.{layout.Name}.Api")
                    .GetResult();

                if (!result.IsSuccessful)
                {
                    failures.Add($"{layout.Name}.Persistence must not depend on its own Api.");
                }
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }
}
