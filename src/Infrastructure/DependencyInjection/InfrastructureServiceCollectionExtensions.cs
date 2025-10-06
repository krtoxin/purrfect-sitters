using Application.Common.Interfaces;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;
public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<ISitterProfileRepository, SitterProfileRepository>();
        services.AddScoped<IOwnerProfileRepository, OwnerProfileRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}