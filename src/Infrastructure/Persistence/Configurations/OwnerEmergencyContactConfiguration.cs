using Domain.Owners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OwnerEmergencyContactConfiguration : IEntityTypeConfiguration<EmergencyContact>
{
    public void Configure(EntityTypeBuilder<EmergencyContact> builder)
    {
        builder.ToTable("owner_emergency_contacts");
        builder.HasKey(x => x.Id).HasName("pk_owner_emergency_contacts");
        
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(150)")
            .IsRequired();
        builder.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasColumnType("varchar(60)")
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property(x => x.OwnerProfileId)
            .HasColumnName("owner_profile_id")
            .IsRequired();
        
        builder.HasIndex(x => x.OwnerProfileId).HasDatabaseName("ix_owner_emergency_contacts_owner_profile_id");
    }
}
