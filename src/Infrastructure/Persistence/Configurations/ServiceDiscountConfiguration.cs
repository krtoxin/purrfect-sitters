using Domain.Sitters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ServiceDiscountConfiguration : IEntityTypeConfiguration<ServiceDiscount>
{
    public void Configure(EntityTypeBuilder<ServiceDiscount> builder)
    {
        builder.ToTable("service_discounts");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Category).HasColumnName("category").HasConversion<int>();
        builder.Property(x => x.Percentage).HasColumnName("percentage").HasColumnType("numeric(5,2)");
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(x => new { x.Category, x.ExpiresAt });
    }
}
