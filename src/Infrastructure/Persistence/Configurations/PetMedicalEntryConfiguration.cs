using Domain.Pets.Medical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PetMedicalEntryConfiguration : IEntityTypeConfiguration<MedicalEntry>
{
    public void Configure(EntityTypeBuilder<MedicalEntry> builder)
    {
        builder.ToTable("pet_medical_entries");
        builder.HasKey(x => x.Id).HasName("pk_pet_medical_entries");
        
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasColumnType("varchar(200)")
            .IsRequired();
        builder.Property(x => x.Details)
            .HasColumnName("details")
            .HasColumnType("varchar(1000)");
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property(x => x.PetId)
            .HasColumnName("pet_id")
            .IsRequired();
        
        builder.HasIndex(x => x.PetId).HasDatabaseName("ix_pet_medical_entries_pet_id");
    }
}
