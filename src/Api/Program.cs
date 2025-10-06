using Api.Modules;
using Application;
using Infrastructure;
using Api.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.SetupServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

await app.InitialiseDatabaseAsync();

app.MapControllers();
app.Run();

public partial class Program { }