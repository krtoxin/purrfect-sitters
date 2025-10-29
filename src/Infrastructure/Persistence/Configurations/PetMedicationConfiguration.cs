using Domain.Pets.Medical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PetMedicationConfiguration : IEntityTypeConfiguration<PetMedication>
{
    public void Configure(EntityTypeBuilder<PetMedication> builder)
    {
        builder.ToTable("pet_medications");
        builder.HasKey(x => x.Id).HasName("pk_pet_medications");
        
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(200)")
            .IsRequired();
        builder.Property(x => x.Dosage)
            .HasColumnName("dosage")
            .HasColumnType("varchar(150)")
            .IsRequired();
        builder.Property(x => x.Schedule)
            .HasColumnName("schedule")
            .HasColumnType("varchar(250)")
            .IsRequired();
        builder.Property(x => x.StartUtc)
            .HasColumnName("start_utc")
            .IsRequired();
        builder.Property(x => x.EndUtc)
            .HasColumnName("end_utc");
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property(x => x.PetId)
            .HasColumnName("pet_id")
            .IsRequired();
        
        builder.HasIndex(x => x.PetId).HasDatabaseName("ix_pet_medications_pet_id");
    }
}
