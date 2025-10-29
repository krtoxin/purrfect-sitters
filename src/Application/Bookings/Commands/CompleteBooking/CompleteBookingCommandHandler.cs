using Application.Common.Interfaces;
using MediatR;

namespace Application.Bookings.Commands.CompleteBooking;

public class CompleteBookingCommandHandler : IRequestHandler<CompleteBookingCommand>
{
    private readonly IBookingRepository _bookings;
    private readonly IUnitOfWork _uow;

    public CompleteBookingCommandHandler(IBookingRepository bookings, IUnitOfWork uow)
    {
        _bookings = bookings;
        _uow = uow;
    }

    public async Task<Unit> Handle(CompleteBookingCommand request, CancellationToken ct)
    {
        var booking = await _bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw new InvalidOperationException("Booking not found.");

        if (!booking.RowVersion.SequenceEqual(request.RowVersion))
            throw new InvalidOperationException("Booking version mismatch (concurrency error).");

        booking.Complete();
        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}