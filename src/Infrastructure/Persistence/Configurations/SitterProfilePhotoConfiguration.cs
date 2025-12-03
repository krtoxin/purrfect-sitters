using Domain.Sitters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SitterProfilePhotoConfiguration : IEntityTypeConfiguration<SitterProfilePhoto>
{
    public void Configure(EntityTypeBuilder<SitterProfilePhoto> builder)
    {
        builder.ToTable("sitter_profile_photos");
        builder.HasKey(x => x.Id).HasName("pk_sitter_profile_photos");
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.SitterProfileId).HasColumnName("sitter_profile_id").IsRequired();
        builder.Property(x => x.Url).HasColumnName("url").HasColumnType("varchar(512)").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("timezone('utc', now())");

        builder.HasOne<SitterProfile>()
            .WithOne()
            .HasForeignKey<SitterProfilePhoto>(x => x.SitterProfileId)
            .HasConstraintName("fk_sitter_profile_photos_sitter_profiles")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.SitterProfileId).IsUnique().HasDatabaseName("ux_sitter_profile_photos_sitter_profile_id");
    }
}
