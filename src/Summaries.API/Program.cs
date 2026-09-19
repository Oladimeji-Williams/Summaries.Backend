using Summaries.API;
using Summaries.API.Cors;
using Summaries.API.Common.Security;
using Summaries.SharedKernel;
using Summaries.SharedKernel.Common.Options;
using Summaries.Shared.Infrastructure;
using Summaries.Modules.Authentication;
using Summaries.Modules.Authentication.Infrastructure.Identity;
using Summaries.Modules.Books;
using Summaries.Modules.Payments;

var envDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
while (envDirectory is not null && !File.Exists(Path.Combine(envDirectory.FullName, ".env")))
{
    envDirectory = envDirectory.Parent;
}

if (envDirectory is null)
{
    throw new FileNotFoundException("The repository .env file was not found.");
}

DotNetEnv.Env.Load(Path.Combine(envDirectory.FullName, ".env"));

var builder = WebApplication.CreateBuilder(args);

var moduleControllerAssemblies = new[]
{
    typeof(Summaries.Modules.Authentication.AssemblyMarker).Assembly,
    typeof(Summaries.Modules.Books.AssemblyMarker).Assembly,
    typeof(Summaries.Modules.Payments.AssemblyMarker).Assembly,
    typeof(Summaries.Modules.Users.AssemblyMarker).Assembly,
    typeof(Summaries.Modules.Admin.AssemblyMarker).Assembly,
};

builder.Services.AddApiServices(builder.Configuration, moduleControllerAssemblies);

// Each module is an independently referenceable class library; the shared
// kernel wires the single MediatR/FluentValidation pipeline across all of them.
builder.Services.AddSharedKernel(moduleControllerAssemblies);

builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddAuthenticationModule(builder.Configuration);
builder.Services.AddBooksModule(builder.Configuration);
builder.Services.AddPaymentsModule(builder.Configuration);

builder.Services.AddMemoryCache();
builder.Services.Configure<FrontendOptions>(builder.Configuration.GetSection(FrontendOptions.SectionName));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseSecurityHeaders();
app.UseStaticFiles();
app.UseApiCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Makes the top-level Program class accessible to
// Summaries.API.IntegrationTests' WebApplicationFactory<Program>.
public partial class Program;
