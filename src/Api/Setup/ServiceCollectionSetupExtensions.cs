using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Setup;

public static class ServiceCollectionSetupExtensions
{
    public static IServiceCollection SetupServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddCors(o =>
        {
            o.AddDefaultPolicy(p =>
                p.AllowAnyOrigin()
                 .AllowAnyHeader()
                 .AllowAnyMethod());
        });

        return services;
    }
}