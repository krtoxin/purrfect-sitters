using Domain.Sitters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SitterProfileServiceConfiguration : IEntityTypeConfiguration<SitterProfileService>
{
    public void Configure(EntityTypeBuilder<SitterProfileService> builder)
    {
        builder.ToTable("sitter_profile_services");
        builder.HasKey(x => new { x.SitterProfileId, x.ServiceId }).HasName("pk_sitter_profile_services");
        builder.Property(x => x.SitterProfileId).HasColumnName("sitter_profile_id");
        builder.Property(x => x.ServiceId).HasColumnName("service_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("timezone('utc', now())");

        builder.HasOne<SitterProfile>()
            .WithMany()
            .HasForeignKey(x => x.SitterProfileId)
            .HasConstraintName("fk_sitter_profile_services_sitter_profiles")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Service>()
            .WithMany()
            .HasForeignKey(x => x.ServiceId)
            .HasConstraintName("fk_sitter_profile_services_services")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ServiceId).HasDatabaseName("ix_sitter_profile_services_service_id");
        builder.HasIndex(x => x.SitterProfileId).HasDatabaseName("ix_sitter_profile_services_sitter_profile_id");
    }
}
