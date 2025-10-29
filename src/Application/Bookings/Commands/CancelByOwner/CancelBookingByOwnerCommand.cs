using Domain.Bookings;
using MediatR;

namespace Application.Bookings.Commands.CancelByOwner;

public sealed record CancelBookingByOwnerCommand(Guid BookingId, BookingCancellationReason Reason) : IRequest;