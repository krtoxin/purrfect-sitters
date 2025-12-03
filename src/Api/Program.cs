using Api.Modules;
using Application;
using Infrastructure;
using Api.Setup;
using DotNetEnv;
using Microsoft.Extensions.Logging;
using Api.Middleware;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Configuration.AddEnvironmentVariables();
if (builder.Environment.EnvironmentName == "Test")
{
    builder.Configuration.AddJsonFile("appsettings.Test.json", optional: true);
}

builder.Services.SetupServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var cs = app.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"[DEBUG] Resolved DefaultConnection: {cs}");

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors();

await app.InitialiseDatabaseAsync();

app.MapControllers();
app.Run();

namespace Api {
    public partial class Program { }
}