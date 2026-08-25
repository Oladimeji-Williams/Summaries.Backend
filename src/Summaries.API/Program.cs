using Summaries.API;
using Summaries.API.Cors;
using Summaries.Application;
using Summaries.Infrastructure;
using Summaries.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseApiCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();