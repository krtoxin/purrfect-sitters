using Domain.Sitters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SitterCommentConfiguration : IEntityTypeConfiguration<SitterComment>
{
    public void Configure(EntityTypeBuilder<SitterComment> builder)
    {
        builder.ToTable("sitter_comments");
        builder.HasKey(x => x.Id).HasName("pk_sitter_comments");

        builder.Property(x => x.SitterProfileId)
            .IsRequired()
            .HasColumnName("sitter_profile_id");

        builder.Property(x => x.Content)
            .IsRequired()
            .HasColumnType("text")
            .HasColumnName("content");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.HasOne<SitterProfile>()
            .WithMany()
            .HasForeignKey(x => x.SitterProfileId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_sitter_comments_sitter_profiles");

        builder.HasIndex(x => x.SitterProfileId)
            .HasDatabaseName("ix_sitter_comments_sitter_profile_id");
    }
}