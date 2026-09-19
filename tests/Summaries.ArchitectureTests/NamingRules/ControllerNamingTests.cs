using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;

using Summaries.ArchitectureTests.Common;

namespace Summaries.ArchitectureTests.NamingRules;

public sealed class ControllerNamingTests
{
    [Fact]
    public void Concrete_Controllers_Should_End_With_Controller()
    {
        var failures = new List<string>();

        // Controllers live in each module's own Api/Controllers folder now,
        // not in a single host assembly — check every module.
        foreach (var (name, assembly) in Assemblies.Modules)
        {
            var result = Types
                .InAssembly(assembly)
                .That()
                .Inherit(typeof(ControllerBase))
                .And()
                .AreNotAbstract()
                .Should()
                .HaveNameEndingWith("Controller")
                .GetResult();

            if (!result.IsSuccessful)
            {
                failures.Add($"{name} module: all concrete API controllers must end with 'Controller'.");
            }
        }

        Assert.True(failures.Count == 0, string.Join(Environment.NewLine, failures));
    }
}
