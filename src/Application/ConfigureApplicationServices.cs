using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using FluentValidation;
using Application.Common.Behaviors;

namespace Application;

public static class ConfigureApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(ConfigureApplicationServices).Assembly;
        services.AddMediatR(assembly);

        RegisterValidatorsManually(services, assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    private static void RegisterValidatorsManually(IServiceCollection services, Assembly assembly)
    {
        var validatorTypes = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Select(t => new
            {
                Impl = t,
                Interfaces = t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))
            })
            .Where(x => x.Interfaces.Any());

        foreach (var vt in validatorTypes)
        {
            foreach (var iface in vt.Interfaces)
            {
                services.AddTransient(iface, vt.Impl);
            }
        }
    }
}