using Domain.Bookings;

namespace Api.Contracts.Bookings.Responses;

public sealed record BookingStatusHistoryResponse(
    Guid Id,
    BookingStatus Status,
    DateTime ChangedAtUtc);