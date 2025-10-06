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

        // Care Instructions
        builder.OwnsMany(p => p.Instructions, b =>
        {
            b.ToTable("pet_instructions");
            b.WithOwner().HasForeignKey("pet_id");
            b.HasKey(x => x.Id).HasName("pk_pet_instructions");
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Text).HasColumnName("text").HasColumnType("varchar(400)").IsRequired();
            b.Property(x => x.CreatedAt).HasColumnName("created_at")
                .HasDefaultValueSql("timezone('utc', now())");
            b.HasIndex("pet_id").HasDatabaseName("ix_pet_instructions_pet_id");
        });

        // Medical Entries
        builder.OwnsMany(p => p.MedicalEntries, b =>
        {
            b.ToTable("pet_medical_entries");
            b.WithOwner().HasForeignKey("pet_id");
            b.HasKey(x => x.Id).HasName("pk_pet_medical_entries");
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Title).HasColumnName("title").HasColumnType("varchar(200)").IsRequired();
            b.Property(x => x.Details).HasColumnName("details").HasColumnType("varchar(1000)");
            b.Property(x => x.CreatedAt).HasColumnName("created_at")
                .HasDefaultValueSql("timezone('utc', now())");
            b.HasIndex("pet_id").HasDatabaseName("ix_pet_medical_entries_pet_id");
        });

        // Allergies
        builder.OwnsMany(p => p.Allergies, b =>
        {
            b.ToTable("pet_allergies");
            b.WithOwner().HasForeignKey("pet_id");
            b.HasKey(x => x.Id).HasName("pk_pet_allergies");
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(200)").IsRequired();
            b.Property(x => x.Severity)
                .HasColumnName("severity")
                .HasConversion<int>()   // enum -> int
                .IsRequired();
            b.Property(x => x.Notes).HasColumnName("notes").HasColumnType("varchar(500)");
            b.Property(x => x.CreatedAt).HasColumnName("created_at")
                .HasDefaultValueSql("timezone('utc', now())");
            b.HasIndex("pet_id").HasDatabaseName("ix_pet_allergies_pet_id");
        });

        // Vaccinations
        builder.OwnsMany(p => p.Vaccinations, b =>
        {
            b.ToTable("pet_vaccinations");
            b.WithOwner().HasForeignKey("pet_id");
            b.HasKey(x => x.Id).HasName("pk_pet_vaccinations");
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.VaccineName).HasColumnName("vaccine_name").HasColumnType("varchar(200)").IsRequired();
            b.Property(x => x.AdministeredOnUtc).HasColumnName("administered_on_utc").IsRequired();
            b.Property(x => x.ExpiresOnUtc).HasColumnName("expires_on_utc");
            b.Property(x => x.CreatedAt).HasColumnName("created_at")
                .HasDefaultValueSql("timezone('utc', now())");
            b.HasIndex("pet_id").HasDatabaseName("ix_pet_vaccinations_pet_id");
        });

        // Medications
        builder.OwnsMany(p => p.Medications, b =>
        {
            b.ToTable("pet_medications");
            b.WithOwner().HasForeignKey("pet_id");
            b.HasKey(x => x.Id).HasName("pk_pet_medications");
            b.Property(x => x.Id).HasColumnName("id");
            b.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(200)").IsRequired();
            b.Property(x => x.Dosage).HasColumnName("dosage").HasColumnType("varchar(150)").IsRequired();
            b.Property(x => x.Schedule).HasColumnName("schedule").HasColumnType("varchar(250)").IsRequired();
            b.Property(x => x.StartUtc).HasColumnName("start_utc").IsRequired();
            b.Property(x => x.EndUtc).HasColumnName("end_utc");
            b.Property(x => x.CreatedAt).HasColumnName("created_at")
                .HasDefaultValueSql("timezone('utc', now())");
            b.HasIndex("pet_id").HasDatabaseName("ix_pet_medications_pet_id");
        });

        // Indexes on main table
        builder.HasIndex(x => x.OwnerId).HasDatabaseName("ix_pets_owner_id");
        builder.HasIndex(x => new { x.OwnerId, x.Type }).HasDatabaseName("ix_pets_owner_type");
    }
}