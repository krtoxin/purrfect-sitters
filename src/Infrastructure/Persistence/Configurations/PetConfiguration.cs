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
        builder.OwnsMany(typeof(CareInstruction), "_instructions", b =>
        {
            b.ToTable("pet_instructions");
            b.WithOwner().HasForeignKey("pet_id");
            b.HasKey("Id").HasName("pk_pet_instructions");
            b.Property<Guid>("Id").HasColumnName("id");
            b.Property<string>("Text").HasColumnName("text").HasColumnType("varchar(400)").IsRequired();
            b.Property<DateTime>("CreatedAt").HasColumnName("created_at")
                .HasDefaultValueSql("timezone('utc', now())");
            b.HasIndex("pet_id").HasDatabaseName("ix_pet_instructions_pet_id");
        });

        // Medical Entries
        builder.OwnsMany(typeof(MedicalEntry), "_medicalEntries", b =>
        {
            b.ToTable("pet_medical_entries");
            b.WithOwner().HasForeignKey("pet_id");
            b.HasKey("Id").HasName("pk_pet_medical_entries");
            b.Property<Guid>("Id").HasColumnName("id");
            b.Property<string>("Title").HasColumnName("title").HasColumnType("varchar(200)").IsRequired();
            b.Property<string?>("Details").HasColumnName("details").HasColumnType("varchar(1000)");
            b.Property<DateTime>("CreatedAt").HasColumnName("created_at")
                .HasDefaultValueSql("timezone('utc', now())");
            b.HasIndex("pet_id").HasDatabaseName("ix_pet_medical_entries_pet_id");
        });

        // Allergies
        builder.OwnsMany(typeof(PetAllergy), "_allergies", b =>
        {
            b.ToTable("pet_allergies");
            b.WithOwner().HasForeignKey("pet_id");
            b.HasKey("Id").HasName("pk_pet_allergies");
            b.Property<Guid>("Id").HasColumnName("id");
            b.Property<string>("Name").HasColumnName("name").HasColumnType("varchar(200)").IsRequired();
            b.Property<int>("Severity").HasColumnName("severity").IsRequired();
            b.Property<string?>("Notes").HasColumnName("notes").HasColumnType("varchar(500)");
            b.Property<DateTime>("CreatedAt").HasColumnName("created_at")
                .HasDefaultValueSql("timezone('utc', now())");
            b.HasIndex("pet_id").HasDatabaseName("ix_pet_allergies_pet_id");
        });

        // Vaccinations
        builder.OwnsMany(typeof(PetVaccination), "_vaccinations", b =>
        {
            b.ToTable("pet_vaccinations");
            b.WithOwner().HasForeignKey("pet_id");
            b.HasKey("Id").HasName("pk_pet_vaccinations");
            b.Property<Guid>("Id").HasColumnName("id");
            b.Property<string>("VaccineName").HasColumnName("vaccine_name").HasColumnType("varchar(200)").IsRequired();
            b.Property<DateTime>("AdministeredOnUtc").HasColumnName("administered_on_utc").IsRequired();
            b.Property<DateTime?>("ExpiresOnUtc").HasColumnName("expires_on_utc");
            b.Property<DateTime>("CreatedAt").HasColumnName("created_at")
                .HasDefaultValueSql("timezone('utc', now())");
            b.HasIndex("pet_id").HasDatabaseName("ix_pet_vaccinations_pet_id");
        });

        // Medications
        builder.OwnsMany(typeof(PetMedication), "_medications", b =>
        {
            b.ToTable("pet_medications");
            b.WithOwner().HasForeignKey("pet_id");
            b.HasKey("Id").HasName("pk_pet_medications");
            b.Property<Guid>("Id").HasColumnName("id");
            b.Property<string>("Name").HasColumnName("name").HasColumnType("varchar(200)").IsRequired();
            b.Property<string>("Dosage").HasColumnName("dosage").HasColumnType("varchar(150)").IsRequired();
            b.Property<string>("Schedule").HasColumnName("schedule").HasColumnType("varchar(250)").IsRequired();
            b.Property<DateTime>("StartUtc").HasColumnName("start_utc").IsRequired();
            b.Property<DateTime?>("EndUtc").HasColumnName("end_utc");
            b.Property<DateTime>("CreatedAt").HasColumnName("created_at")
                .HasDefaultValueSql("timezone('utc', now())");
            b.HasIndex("pet_id").HasDatabaseName("ix_pet_medications_pet_id");
        });

        builder.HasIndex(x => x.OwnerId).HasDatabaseName("ix_pets_owner_id");
        builder.HasIndex(x => new { x.OwnerId, x.Type }).HasDatabaseName("ix_pets_owner_type");
    }
}