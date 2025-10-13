using Domain.Pets;
using Domain.Pets.Medical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("pets");
        builder.HasKey(x => x.Id).HasName("pk_pets");

        builder.Property(x => x.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(150)")
            .IsRequired();

        builder.Property(x => x.Breed)
            .HasColumnName("breed")
            .HasColumnType("varchar(150)")
            .IsRequired(false);

        builder.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasColumnType("varchar(500)")
            .IsRequired(false);

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasColumnName("type")
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");

        builder.HasIndex(x => x.OwnerId).HasDatabaseName("ix_pets_owner_id");
        builder.HasIndex(x => new { x.OwnerId, x.Type }).HasDatabaseName("ix_pets_owner_type");
    }
}