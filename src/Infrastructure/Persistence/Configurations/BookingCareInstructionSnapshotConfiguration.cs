using Domain.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BookingCareInstructionSnapshotConfiguration : IEntityTypeConfiguration<BookingCareInstructionSnapshot>
{
    public void Configure(EntityTypeBuilder<BookingCareInstructionSnapshot> builder)
    {
        builder.ToTable("booking_care_instruction_snapshots");
        builder.HasKey(x => x.Id).HasName("pk_booking_care_instruction_snapshots");
        
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Text)
            .HasColumnName("text")
            .HasColumnType("varchar(400)")
            .IsRequired();
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property("BookingId")
            .HasColumnName("booking_id")
            .IsRequired();
        
        builder.HasIndex("booking_id").HasDatabaseName("ix_booking_care_instruction_snapshots_booking_id");
    }
}
