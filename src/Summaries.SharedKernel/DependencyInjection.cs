using System.Reflection;

using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Summaries.SharedKernel.Behaviors;

namespace Summaries.SharedKernel;

/// <summary>
/// Each feature module (Authentication, Books, Payments, Users, Admin) ships
/// as its own class library and owns its own MediatR requests/handlers/
/// validators, but they all share a single MediatR pipeline and validation
/// behavior. Wiring that up once here — instead of once per module — avoids
/// registering the same open pipeline behavior multiple times.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddSharedKernel(
        this IServiceCollection services,
        params Assembly[] moduleAssemblies)
    {
        services.AddMediatR(configuration =>
        {
            foreach (var assembly in moduleAssemblies)
            {
                configuration.RegisterServicesFromAssembly(assembly);
            }

            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        foreach (var assembly in moduleAssemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }

        return services;
    }
}
