using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;

namespace Application;

public static class ConfigureApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(typeof(ConfigureApplicationServices).Assembly);

        return services;
    }
}