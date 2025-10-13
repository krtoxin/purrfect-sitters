using Domain.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BookingStatusHistoryConfiguration : IEntityTypeConfiguration<BookingStatusHistory>
{
    public void Configure(EntityTypeBuilder<BookingStatusHistory> builder)
    {
        builder.ToTable("booking_status_history");
        builder.HasKey(x => x.Id).HasName("pk_booking_status_history");
        
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>();
        builder.Property(x => x.ChangedAtUtc).HasColumnName("changed_at_utc");
        
        builder.Property("BookingId")
            .HasColumnName("booking_id")
            .IsRequired();
        
        builder.HasIndex("booking_id").HasDatabaseName("ix_booking_status_history_booking_id");
    }
}
