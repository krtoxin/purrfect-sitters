using Domain.Sitters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SitterAvailabilitySlotConfiguration : IEntityTypeConfiguration<AvailabilitySlot>
{
    public void Configure(EntityTypeBuilder<AvailabilitySlot> builder)
    {
        builder.ToTable("sitter_availability_slots");
        builder.HasKey(x => x.Id).HasName("pk_sitter_availability_slots");
        
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.StartUtc)
            .HasColumnName("start_utc")
            .IsRequired();
        builder.Property(x => x.EndUtc)
            .HasColumnName("end_utc")
            .IsRequired();
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property("SitterProfileId")
            .HasColumnName("sitter_profile_id")
            .IsRequired();
        
        builder.HasIndex("sitter_profile_id").HasDatabaseName("ix_sitter_availability_slots_profile_id");
        builder.HasIndex("StartUtc", "EndUtc").HasDatabaseName("ix_sitter_availability_slots_range");
    }
}
