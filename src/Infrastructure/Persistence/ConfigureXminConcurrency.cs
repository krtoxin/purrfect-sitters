using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence;

public static class ConfigureXminConcurrency
{
    public static void MapXminConcurrency<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class
    {
        builder.Property<uint>("xmin")
            .HasColumnName("xmin")
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();
    }
    public static void ApplyXminConcurrency(this ModelBuilder modelBuilder)
    {
        // Only map xmin for Booking at runtime
        modelBuilder.Entity<Domain.Bookings.Booking>(b => b.MapXminConcurrency());
    }
}