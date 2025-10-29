using Domain.Sitters;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SitterConfiguration : IEntityTypeConfiguration<Sitter>
{
    public void Configure(EntityTypeBuilder<Sitter> builder)
    {
        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(a => a.Line1).HasColumnName("address_line1");
            address.Property(a => a.Line2).HasColumnName("address_line2");
            address.Property(a => a.City).HasColumnName("address_city");
            address.Property(a => a.StateOrProvince).HasColumnName("address_state");
            address.Property(a => a.PostalCode).HasColumnName("address_postal_code");
            address.Property(a => a.Country).HasColumnName("address_country");
        });

        builder.OwnsOne(x => x.EmailValue, email =>
        {
            email.Property(e => e.Value).HasColumnName("email");
        });

        builder.OwnsOne(x => x.HourlyRate, money =>
        {
            money.Property(m => m.Amount).HasColumnName("hourly_rate_amount");
            money.Property(m => m.Currency).HasColumnName("hourly_rate_currency");
        });
    }
}
