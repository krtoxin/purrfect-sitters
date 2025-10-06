using MediatR;

namespace Application.Bookings.Commands.CompleteBooking;

public sealed record CompleteBookingCommand(Guid BookingId) : IRequest;