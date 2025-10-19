using Domain.Pets.Medical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PetAllergyConfiguration : IEntityTypeConfiguration<PetAllergy>
{
    public void Configure(EntityTypeBuilder<PetAllergy> builder)
    {
        builder.ToTable("pet_allergies");
        builder.HasKey(x => x.Id).HasName("pk_pet_allergies");
        
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(200)")
            .IsRequired();
        builder.Property(x => x.Severity)
            .HasColumnName("severity")
            .HasConversion<int>()
            .IsRequired();
        builder.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasColumnType("varchar(500)");
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property(x => x.PetId)
            .HasColumnName("pet_id")
            .IsRequired();
        
        builder.HasIndex(x => x.PetId).HasDatabaseName("ix_pet_allergies_pet_id");
    }
}
