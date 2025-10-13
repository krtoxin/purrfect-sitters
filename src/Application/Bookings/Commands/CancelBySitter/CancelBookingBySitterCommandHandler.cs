using Application.Common.Interfaces;
using MediatR;

namespace Application.Bookings.Commands.CancelBySitter;

public class CancelBookingBySitterCommandHandler : IRequestHandler<CancelBookingBySitterCommand>
{
    private readonly IBookingRepository _bookings;
    private readonly IUnitOfWork _uow;

    public CancelBookingBySitterCommandHandler(IBookingRepository bookings, IUnitOfWork uow)
    {
        _bookings = bookings;
        _uow = uow;
    }

    public async Task<Unit> Handle(CancelBookingBySitterCommand request, CancellationToken ct)
    {
        var booking = await _bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw new InvalidOperationException("Booking not found.");

        booking.CancelBySitter(request.Reason);
        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}