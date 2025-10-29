using System.Reflection;
using Domain.Users;
using Domain.Owners;
using Domain.Sitters;
using Domain.Pets;
using Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; init; }
    public DbSet<OwnerProfile> OwnerProfiles { get; init; }
    public DbSet<SitterProfile> SitterProfiles { get; init; }
    public DbSet<Pet> Pets { get; init; }
    public DbSet<Booking> Bookings { get; init; }
    public DbSet<Sitter> Sitters { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Relational:HistoryTableName", "__EFMigrationsHistory");
        modelBuilder.HasAnnotation("Relational:HistoryTableSchema", "public");
        modelBuilder.HasAnnotation("Relational:HistoryTableMigrationIdColumnName", "migration_id");
        modelBuilder.HasAnnotation("Relational:HistoryTableProductVersionColumnName", "product_version");
        // Use the assembly that defines the DbContext (Infrastructure) to ensure
        // that model configuration is consistent between design-time (migrations)
        // and runtime (tests/production). Assembly.GetExecutingAssembly() can
        // return a different assembly when the context is constructed from tests.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        // Always map xmin at runtime (including tests)
        modelBuilder.ApplyXminConcurrency();
        base.OnModelCreating(modelBuilder);
    }
}