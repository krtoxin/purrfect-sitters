using System.Reflection;
using Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Api.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        var appAssembly = typeof(ApplicationAssemblyMarker).Assembly;

        services.AddMediatR(appAssembly);

        if (HasFluentValidationDiExtension())
        {
            services.AddValidatorsFromAssembly(appAssembly, includeInternalTypes: true);
        }
        else
        {
            RegisterValidatorsManually(services, appAssembly);
        }

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    private static bool HasFluentValidationDiExtension()
    {
        var extType = Type.GetType("FluentValidation.DependencyInjectionExtensions.ServiceCollectionExtensions, FluentValidation.DependencyInjectionExtensions");
        return extType?.GetMethod("AddValidatorsFromAssembly", BindingFlags.Public | BindingFlags.Static) != null;
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

public interface ApplicationAssemblyMarker {}