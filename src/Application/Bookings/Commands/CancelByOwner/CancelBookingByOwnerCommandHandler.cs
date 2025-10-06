using Application.Common.Interfaces;
using MediatR;

namespace Application.Bookings.Commands.CancelByOwner;

public class CancelBookingByOwnerCommandHandler : IRequestHandler<CancelBookingByOwnerCommand>
{
    private readonly IBookingRepository _bookings;
    private readonly IUnitOfWork _uow;

    public CancelBookingByOwnerCommandHandler(IBookingRepository bookings, IUnitOfWork uow)
    {
        _bookings = bookings;
        _uow = uow;
    }

    public async Task Handle(CancelBookingByOwnerCommand request, CancellationToken ct)
    {
        var booking = await _bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw new InvalidOperationException("Booking not found.");

        booking.CancelByOwner(request.Reason);
        await _uow.SaveChangesAsync(ct);
    }
}