using Domain.Bookings;

namespace Application.Bookings.Models;

public sealed record BookingPriceReadModel(
    decimal BaseAmount,
    decimal? ServiceFeePercent,
    decimal ServiceFeeAmount,
    decimal TotalAmount,
    string Currency);

public sealed record BookingStatusHistoryReadModel(
    Guid Id,
    BookingStatus Status,
    DateTime ChangedAtUtc);

public sealed record BookingCareInstructionReadModel(
    Guid Id,
    string Text,
    DateTime CreatedAt);

public sealed record BookingReadModel(
    Guid Id,
    Guid PetId,
    Guid OwnerId,
    Guid SitterProfileId,
    DateTime StartUtc,
    DateTime EndUtc,
    BookingStatus Status,
    BookingPriceReadModel Price,
    bool IsReviewed,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? CompletedAtUtc,
    DateTime? CancelledAtUtc,
    BookingCancellationReason? CancellationReason,
    byte[] RowVersion,
    IEnumerable<BookingStatusHistoryReadModel> StatusHistory,
    IEnumerable<BookingCareInstructionReadModel> CareInstructions);