using Api.Contracts.Bookings.Responses;
using Application.Bookings.Models;

namespace Api.Mappings;

public static class BookingApiMappings
{
    public static BookingResponse ToResponse(this BookingReadModel model)
    {
        var price = new BookingPriceResponse(
            model.Price.BaseAmount,
            model.Price.ServiceFeePercent,
            model.Price.ServiceFeeAmount,
            model.Price.TotalAmount,
            model.Price.Currency);

        var history = model.StatusHistory
            .Select(h => new BookingStatusHistoryResponse(h.Id, h.Status, h.ChangedAtUtc))
            .ToList();

        var care = model.CareInstructions
            .Select(c => new BookingCareInstructionResponse(c.Id, c.Text, c.CreatedAt))
            .ToList();

        return new BookingResponse(
            model.Id,
            model.PetId,
            model.OwnerId,
            model.SitterProfileId,
            model.StartUtc,
            model.EndUtc,
            model.Status.ToString(),
            price,
            model.IsReviewed,
            model.CreatedAt,
            model.UpdatedAt,
            model.CompletedAtUtc,
            model.CancelledAtUtc,
            model.CancellationReason,
            // removed RowVersion,
            history,
            care);
    }
}