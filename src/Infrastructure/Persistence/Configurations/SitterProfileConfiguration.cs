using Domain.Sitters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SitterProfileConfiguration : IEntityTypeConfiguration<SitterProfile>
{
    public void Configure(EntityTypeBuilder<SitterProfile> builder)
    {
        builder.ToTable("sitter_profiles");
        builder.HasKey(x => x.Id).HasName("pk_sitter_profiles");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.Bio)
            .HasColumnName("bio")
            .HasColumnType("varchar(800)")
            .IsRequired(false);

        builder.Property(x => x.AverageRating)
            .HasColumnName("average_rating")
            .HasPrecision(4, 2)
            .HasDefaultValue(0m);

        builder.Property(x => x.CompletedBookings)
            .HasColumnName("completed_bookings")
            .HasDefaultValue(0);

        builder.Property(x => x.ServicesOffered)
            .HasColumnName("services_offered")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");

        builder.OwnsOne(x => x.BaseRate, m =>
        {
            m.Property(p => p.Amount).HasColumnName("base_rate_amount").HasPrecision(12, 2);
            m.Property(p => p.Currency).HasColumnName("base_rate_currency").HasColumnType("varchar(3)");
        });


        builder.HasIndex(x => x.UserId).HasDatabaseName("ix_sitter_profiles_user_id");
        builder.HasIndex(x => x.IsActive).HasDatabaseName("ix_sitter_profiles_is_active");
    }
}