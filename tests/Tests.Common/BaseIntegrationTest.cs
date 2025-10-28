using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;

namespace Tests.Common;

public interface ITestWebAppFactory
{
    HttpClient CreateClient();
    IServiceScope CreateScope();
}

public abstract class BaseIntegrationTest
{
    protected readonly HttpClient Client;
    protected readonly ApplicationDbContext Context;

    protected BaseIntegrationTest(ITestWebAppFactory factory)
    {
        Client = factory.CreateClient();
        var scope = factory.CreateScope();
        Context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    protected async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }
}
