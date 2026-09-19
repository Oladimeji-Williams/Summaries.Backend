using System.Reflection;

namespace Summaries.ArchitectureTests.Common;

public static class Assemblies
{
    public static Assembly SharedKernel =>
        typeof(Summaries.SharedKernel.Common.Primitives.Result).Assembly;

    public static Assembly SharedInfrastructure =>
        typeof(Summaries.Shared.Infrastructure.DependencyInjection).Assembly;

    public static Assembly Authentication =>
        typeof(Summaries.Modules.Authentication.AssemblyMarker).Assembly;

    public static Assembly Books =>
        typeof(Summaries.Modules.Books.AssemblyMarker).Assembly;

    public static Assembly Payments =>
        typeof(Summaries.Modules.Payments.AssemblyMarker).Assembly;

    public static Assembly Users =>
        typeof(Summaries.Modules.Users.AssemblyMarker).Assembly;

    public static Assembly Admin =>
        typeof(Summaries.Modules.Admin.AssemblyMarker).Assembly;

    public static (string Name, Assembly Assembly)[] Modules =>
    [
        (nameof(Authentication), Authentication),
        (nameof(Books), Books),
        (nameof(Payments), Payments),
        (nameof(Users), Users),
        (nameof(Admin), Admin),
    ];

    public static Assembly Api =>
        typeof(Summaries.API.DependencyInjection).Assembly;
}
