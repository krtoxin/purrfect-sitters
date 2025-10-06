using MediatR;

namespace Application.Bookings.Commands.AcceptBooking;

public sealed record AcceptBookingCommand(Guid BookingId) : IRequest;