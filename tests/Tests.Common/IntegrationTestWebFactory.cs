using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        if (!_dbContainer.State.Equals(DotNet.Testcontainers.Containers.TestcontainersStates.Running))
        {
            _dbContainer.StartAsync().GetAwaiter().GetResult();
        }

        builder.UseEnvironment("Development");

        builder.ConfigureLogging((context, logging) =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });

        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", _dbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _dbContainer.GetConnectionString()
            };
            configBuilder.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            RegisterDatabase(services);

            services.AddSingleton<Application.Common.Interfaces.IEmailSendingService, Tests.Common.Services.DummyEmailSendingService>();

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IntegrationTestWebFactory>>();
            try
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                logger.LogInformation("Resetting test database...");
                db.Database.EnsureDeletedAsync().GetAwaiter().GetResult();

                logger.LogInformation("Applying EF Core migrations for test schema...");
                db.Database.MigrateAsync().GetAwaiter().GetResult();
                logger.LogInformation("Test schema created via migrations.");

                logger.LogInformation("Test database ready.");
            }
            catch (Exception ex)
            {
                var lf = sp.GetRequiredService<ILoggerFactory>();
                lf.CreateLogger("IntegrationTestWebFactory").LogError(ex, "Failed to initialize test database during ConfigureTestServices");
                throw;
            }
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
            .ConfigureWarnings(w => {
                w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                w.Ignore(RelationalEventId.PendingModelChangesWarning);
            }));
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