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
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("test-purrfect-sitters-database")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Ensure the container is started before using its connection string
        if (!_dbContainer.State.Equals(DotNet.Testcontainers.Containers.TestcontainersStates.Running))
        {
            _dbContainer.StartAsync().GetAwaiter().GetResult();
        }

        // Set environment variable to override connection string for all config providers
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _dbContainer.GetConnectionString()
            };
            // Add as first provider so it overrides appsettings.json but can be overridden by env vars
            configBuilder.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<Application.Common.Interfaces.IEmailSendingService, Tests.Common.Services.DummyEmailSendingService>();
        });
    }

    private void RegisterDatabase(IServiceCollection services)
    {
        services.RemoveServiceByType(typeof(DbContextOptions<ApplicationDbContext>));

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(_dbContainer.GetConnectionString());
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ApplicationDbContext>(options => options
            .UseNpgsql(
                dataSource,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            .UseSnakeCaseNamingConvention()
            .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)));
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.DisposeAsync().AsTask();
    }
}