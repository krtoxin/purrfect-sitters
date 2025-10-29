using MediatR;

namespace Application.Bookings.Commands.DeleteBooking;

public sealed record DeleteBookingCommand(
    Guid Id
) : IRequest<bool>;
