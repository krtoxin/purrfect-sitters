using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Infrastructure.Persistence;

public class ApplicationDbContextInitialiser
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;

    public ApplicationDbContextInitialiser(
        ApplicationDbContext context,
        ILogger<ApplicationDbContextInitialiser> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitialiseAsync(CancellationToken ct = default)
    {
        try
        {
            var pending = await _context.Database.GetPendingMigrationsAsync(ct);
            if (pending.Any())
            {
                _logger.LogInformation("Applying {Count} migrations...", pending.Count());
                await _context.Database.MigrateAsync(ct);
            }
            await SeedAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialisation failed");
            throw;
        }
    }

    private Task SeedAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}