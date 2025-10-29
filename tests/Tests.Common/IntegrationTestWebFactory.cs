using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;
using Xunit;

namespace Tests.Common;


public class IntegrationTestWebFactory : WebApplicationFactory<Api.Program>, IAsyncLifetime
{
    private const string NeonDbConnectionString = "Host=ep-holy-field-agyc3h7m-pooler.c-2.eu-central-1.aws.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_PfWg5hHm1MzZ;SSL Mode=Require;Trust Server Certificate=true";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureTestServices(services =>
        {
            RegisterDatabase(services);
            RegisterTestServices(services);
        })
        .ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile("appsettings.Test.json");
            config.AddEnvironmentVariables();
        });
    }

    private void RegisterTestServices(IServiceCollection services)
    {
        services.RemoveServiceByType(typeof(Application.Common.Interfaces.IEmailSendingService));
        services.AddScoped<Application.Common.Interfaces.IEmailSendingService, Tests.Common.Services.DummyEmailSendingService>();
    }

    private void RegisterDatabase(IServiceCollection services)
    {
        services.RemoveServiceByType(typeof(DbContextOptions<ApplicationDbContext>));

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(NeonDbConnectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ApplicationDbContext>(options => options
            .UseNpgsql(
                dataSource,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            .UseSnakeCaseNamingConvention()
            .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)));
    }



    public Task InitializeAsync() => Task.CompletedTask;
    public new Task DisposeAsync() => Task.CompletedTask;
}