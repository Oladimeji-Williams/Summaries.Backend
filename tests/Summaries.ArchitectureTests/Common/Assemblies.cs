using System.Reflection;

using Summaries.API;
using Summaries.Application;
using Summaries.Domain;
using Summaries.Infrastructure;
using Summaries.Persistence;

namespace Summaries.ArchitectureTests.Common;

public static class Assemblies
{
    public static Assembly Domain =>
        typeof(Summaries.Domain.Entities.Book).Assembly;

    public static Assembly Application =>
        typeof(Summaries.Application.Features.Books.Shared.DTOs.BookDto).Assembly;

    public static Assembly Persistence =>
        typeof(Summaries.Persistence.Context.ApplicationDbContext).Assembly;

    public static Assembly Infrastructure =>
        typeof(Summaries.Infrastructure.DependencyInjection).Assembly;

    public static Assembly Api =>
        typeof(Summaries.API.Controllers.V1.BooksController).Assembly;
}