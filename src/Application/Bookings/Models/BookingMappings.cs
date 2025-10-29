using Domain.Bookings;

namespace Application.Bookings.Models;

public static class BookingMappings
{
    public static BookingReadModel ToReadModel(this Booking b)
    {
        var price = new BookingPriceReadModel(
            b.Price.BaseAmount,
            b.Price.ServiceFeePercent,
            b.Price.ServiceFeeAmount,
            b.Price.TotalAmount,
            b.Price.Currency);

        var history = b.StatusHistory
            .OrderBy(h => h.ChangedAtUtc)
            .Select(h => new BookingStatusHistoryReadModel(h.Id, h.Status, h.ChangedAtUtc))
            .ToList();

        var care = b.CareInstructionSnapshots
            .OrderBy(c => c.CreatedAt)
            .Select(c => new BookingCareInstructionReadModel(c.Id, c.Text, c.CreatedAt))
            .ToList();

        return new BookingReadModel(
            b.Id,
            b.PetId,
            b.OwnerId,
            b.SitterProfileId,
            b.StartUtc,
            b.EndUtc,
            b.Status,
            price,
            b.IsReviewed,
            b.CreatedAt,
            b.UpdatedAt,
            b.CompletedAtUtc,
            b.CancelledAtUtc,
            b.CancellationReason,
            b.RowVersion,
            history,
            care);
    }
}