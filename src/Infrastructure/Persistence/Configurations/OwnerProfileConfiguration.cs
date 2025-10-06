using Domain.Owners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OwnerProfileConfiguration : IEntityTypeConfiguration<OwnerProfile>
{
    public void Configure(EntityTypeBuilder<OwnerProfile> builder)
    {
        builder.ToTable("owner_profiles");
        builder.HasKey(x => x.Id).HasName("pk_owner_profiles");

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.DefaultCareNotes)
            .HasColumnName("default_care_notes")
            .HasColumnType("varchar(800)")
            .IsRequired(false);

        builder.Property(x => x.PreferredTimezone)
            .HasColumnName("preferred_timezone")
            .HasColumnType("varchar(100)")
            .IsRequired(false);

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.OwnsOne(x => x.Address, a =>
        {
            a.Property(p => p.Line1).HasColumnName("address_line1").HasColumnType("varchar(200)");
            a.Property(p => p.Line2).HasColumnName("address_line2").HasColumnType("varchar(200)");
            a.Property(p => p.City).HasColumnName("address_city").HasColumnType("varchar(120)");
            a.Property(p => p.StateOrProvince).HasColumnName("address_state").HasColumnType("varchar(120)");
            a.Property(p => p.PostalCode).HasColumnName("address_postal_code").HasColumnType("varchar(40)");
            a.Property(p => p.Country).HasColumnName("address_country").HasColumnType("varchar(80)");
        });

        builder.OwnsMany(x => x.EmergencyContacts, b =>
        {
            b.ToTable("owner_emergency_contacts");
            b.WithOwner().HasForeignKey("owner_profile_id");
            b.HasKey(x => x.Id).HasName("pk_owner_emergency_contacts");
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(150)").IsRequired();
            b.Property(x => x.Phone).HasColumnName("phone").HasColumnType("varchar(60)").IsRequired();
            b.Property(x => x.CreatedAt).HasColumnName("created_at")
                .HasDefaultValueSql("timezone('utc', now())");
            b.HasIndex("owner_profile_id").HasDatabaseName("ix_owner_emergency_contacts_owner_profile_id");
        });

        builder.HasIndex(x => x.UserId).HasDatabaseName("ix_owner_profiles_user_id");
        builder.HasIndex(x => x.IsActive).HasDatabaseName("ix_owner_profiles_is_active");
    }
}