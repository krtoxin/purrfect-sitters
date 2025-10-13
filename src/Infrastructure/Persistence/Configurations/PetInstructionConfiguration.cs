using Domain.Pets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PetInstructionConfiguration : IEntityTypeConfiguration<CareInstruction>
{
    public void Configure(EntityTypeBuilder<CareInstruction> builder)
    {
        builder.ToTable("pet_instructions");
        builder.HasKey(x => x.Id).HasName("pk_pet_instructions");
        
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Text)
            .HasColumnName("text")
            .HasColumnType("varchar(400)")
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property("PetId")
            .HasColumnName("pet_id")
            .IsRequired();
        
        builder.HasIndex("pet_id").HasDatabaseName("ix_pet_instructions_pet_id");
    }
}
