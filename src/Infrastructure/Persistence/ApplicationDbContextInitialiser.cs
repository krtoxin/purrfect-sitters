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
            try
            {
                var pending = await _context.Database.GetPendingMigrationsAsync(ct);
                if (pending.Any())
                {
                    _logger.LogInformation("Applying {Count} migrations...", pending.Count());
                    await _context.Database.MigrateAsync(ct);
                }
            }
            catch (PostgresException pex) when (pex.SqlState == "42703" || (pex.Message?.Contains("migration_id", StringComparison.OrdinalIgnoreCase) ?? false))
            {
                _logger.LogWarning(pex, "Postgres error while reading __EFMigrationsHistory (missing column or different schema). Falling back to EnsureCreated for tests/local.");
                await _context.Database.EnsureCreatedAsync(ct);
            }
            catch (InvalidOperationException iox) when (iox.Message != null && iox.Message.Contains("The model for context", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(iox, "Migrations cannot be applied because the model has changes that are not represented by migrations. Falling back to EnsureCreated for tests/local.");
                await _context.Database.EnsureCreatedAsync(ct);
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