using Domain.Bookings;

namespace Api.Contracts.Bookings.Requests;

public sealed record CancelBookingRequest(BookingCancellationReason Reason);