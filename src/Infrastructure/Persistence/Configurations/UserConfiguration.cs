using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id).HasName("pk_users");
        builder.Property(x => x.Id).HasColumnName("id");

        builder.OwnsOne(x => x.Email, b =>
        {
            b.Property(e => e.Value)
                .HasColumnName("email")
                .HasColumnType("varchar(255)")
                .IsRequired();

            b.HasIndex(e => e.Value)
                .IsUnique()
                .HasDatabaseName("ux_users_email");
        });

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(x => x.Roles)
            .HasColumnName("roles")
            .IsRequired();

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");
    }
}