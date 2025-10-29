using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Persistence;

public static class ConfigurePersistenceServices
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {

        var cs = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(cs))
        {
            cs = Environment.GetEnvironmentVariable("TEST_DB_CONNECTION");
        }
        if (string.IsNullOrEmpty(cs))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(cs);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ApplicationDbContext>(options => 
        {
            options.UseNpgsql(
                dataSource,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

            options.ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));
        });

        services.AddScoped<ApplicationDbContextInitialiser>();
    }
}