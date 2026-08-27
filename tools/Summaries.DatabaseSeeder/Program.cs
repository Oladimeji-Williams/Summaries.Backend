using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Summaries.DatabaseSeeder;
using Summaries.Infrastructure;
using Summaries.Persistence;

var backendRoot = Path.GetFullPath(
    Path.Combine(
        AppContext.BaseDirectory,
        "..", "..", "..", "..", ".."));
var apiPath = Path.Combine(backendRoot, "src", "Summaries.API");

var configuration =
    new ConfigurationBuilder()
        .SetBasePath(apiPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
        .AddEnvironmentVariables()
        .Build();

var services = new ServiceCollection();
services.AddPersistence(configuration);
services.AddInfrastructure(configuration);

await using var serviceProvider = services.BuildServiceProvider();
await DatabaseInitializer.InitializeAsync(serviceProvider);

Console.WriteLine();
Console.WriteLine("Database reset and seeded successfully.");