using Domain.Sitters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services");
        builder.HasKey(x => x.Id).HasName("pk_services");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name").HasColumnType("varchar(64)").IsRequired();
        builder.HasIndex(x => x.Name).IsUnique().HasDatabaseName("ux_services_name");
    }
}
