using Domain.Pets.Medical;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PetVaccinationConfiguration : IEntityTypeConfiguration<PetVaccination>
{
    public void Configure(EntityTypeBuilder<PetVaccination> builder)
    {
        builder.ToTable("pet_vaccinations");
        builder.HasKey(x => x.Id).HasName("pk_pet_vaccinations");
        
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.VaccineName)
            .HasColumnName("vaccine_name")
            .HasColumnType("varchar(200)")
            .IsRequired();
        builder.Property(x => x.AdministeredOnUtc)
            .HasColumnName("administered_on_utc")
            .IsRequired();
        builder.Property(x => x.ExpiresOnUtc)
            .HasColumnName("expires_on_utc");
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property("PetId")
            .HasColumnName("pet_id")
            .IsRequired();
        
        builder.HasIndex("pet_id").HasDatabaseName("ix_pet_vaccinations_pet_id");
    }
}
