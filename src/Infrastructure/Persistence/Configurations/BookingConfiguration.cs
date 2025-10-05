using Domain.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("bookings");
        builder.HasKey(x => x.Id).HasName("pk_bookings");

        builder.Property(x => x.PetId).HasColumnName("pet_id").IsRequired();
        builder.Property(x => x.OwnerId).HasColumnName("owner_id").IsRequired();
        builder.Property(x => x.SitterProfileId).HasColumnName("sitter_profile_id").IsRequired();

        builder.Property(x => x.StartUtc)
            .HasColumnName("start_utc")
            .IsRequired();

        builder.Property(x => x.EndUtc)
            .HasColumnName("end_utc")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(x => x.ServiceType)
            .HasColumnName("service_type")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(x => x.CompletedAtUtc)
            .HasColumnName("completed_at_utc");

        builder.Property(x => x.CancelledAtUtc)
            .HasColumnName("cancelled_at_utc");

        builder.Property(x => x.CancellationReason)
            .HasColumnName("cancellation_reason");

        builder.Property(x => x.IsReviewed)
            .HasColumnName("is_reviewed")
            .HasDefaultValue(false);

        builder.Property(x => x.Version)
            .HasColumnName("version")
            .IsConcurrencyToken();

        // Price (owned value object)
        builder.OwnsOne(x => x.Price, b =>
        {
            b.Property(p => p.BaseAmount)
                .HasColumnName("price_base_amount")
                .HasPrecision(12, 2);

            b.Property(p => p.ServiceFeePercent)
                .HasColumnName("price_service_fee_percent")
                .HasPrecision(5, 2);

            b.Property(p => p.ServiceFeeAmount)
                .HasColumnName("price_service_fee_amount")
                .HasPrecision(12, 2);

            b.Property(p => p.TotalAmount)
                .HasColumnName("price_total_amount")
                .HasPrecision(12, 2);

            b.Property(p => p.Currency)
                .HasColumnName("price_currency")
                .HasColumnType("varchar(3)");
        });

        // Status history
        builder.OwnsMany(typeof(BookingStatusHistory), "_statusHistory", b =>
        {
            b.ToTable("booking_status_history");
            b.WithOwner().HasForeignKey("booking_id");
            b.HasKey("Id").HasName("pk_booking_status_history");
            b.Property<Guid>("Id").HasColumnName("id");
            b.Property<int>("Status").HasColumnName("status");
            b.Property<DateTime>("ChangedAtUtc").HasColumnName("changed_at_utc");
            b.HasIndex("booking_id").HasDatabaseName("ix_booking_status_history_booking_id");
        });

        // Care instruction snapshots
        builder.OwnsMany(typeof(BookingCareInstructionSnapshot), "_careInstructionSnapshots", b =>
        {
            b.ToTable("booking_care_instruction_snapshots");
            b.WithOwner().HasForeignKey("booking_id");
            b.HasKey("Id").HasName("pk_booking_care_instruction_snapshots");
            b.Property<Guid>("Id").HasColumnName("id");
            b.Property<string>("Text").HasColumnName("text").HasColumnType("varchar(400)").IsRequired();
            b.Property<DateTime>("CreatedAt").HasColumnName("created_at").HasDefaultValueSql("timezone('utc', now())");
            b.HasIndex("booking_id").HasDatabaseName("ix_booking_care_instruction_snapshots_booking_id");
        });

        // Useful indexes
        builder.HasIndex(x => x.PetId).HasDatabaseName("ix_bookings_pet_id");
        builder.HasIndex(x => x.Status).HasDatabaseName("ix_bookings_status");
        builder.HasIndex(x => new { x.SitterProfileId, x.StartUtc, x.EndUtc })
            .HasDatabaseName("ix_bookings_sitter_period");
        builder.HasIndex(x => new { x.OwnerId, x.CreatedAt })
            .HasDatabaseName("ix_bookings_owner_created");
    }
}