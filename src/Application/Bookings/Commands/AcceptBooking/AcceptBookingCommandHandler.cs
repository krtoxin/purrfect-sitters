using Application.Common.Interfaces;
using MediatR;

namespace Application.Bookings.Commands.AcceptBooking;

public class AcceptBookingCommandHandler : IRequestHandler<AcceptBookingCommand>
{
    private readonly IBookingRepository _bookings;
    private readonly IUnitOfWork _uow;

    public AcceptBookingCommandHandler(IBookingRepository bookings, IUnitOfWork uow)
    {
        _bookings = bookings;
        _uow = uow;
    }

    public async Task Handle(AcceptBookingCommand request, CancellationToken ct)
    {
        var booking = await _bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw new InvalidOperationException("Booking not found.");

        booking.Accept();
        await _uow.SaveChangesAsync(ct);
    }
}