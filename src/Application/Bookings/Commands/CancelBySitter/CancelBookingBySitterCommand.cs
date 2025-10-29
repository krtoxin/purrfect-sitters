using Domain.Bookings;
using MediatR;

namespace Application.Bookings.Commands.CancelBySitter;

public sealed record CancelBookingBySitterCommand(Guid BookingId, BookingCancellationReason Reason) : IRequest;