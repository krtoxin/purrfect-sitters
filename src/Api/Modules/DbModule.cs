using Infrastructure.Persistence;

namespace Api.Modules;

public static class DbModule
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var init = scope.ServiceProvider.GetService<ApplicationDbContextInitialiser>();
        if (init is not null)
        {
            await init.InitialiseAsync();
        }
    }
}