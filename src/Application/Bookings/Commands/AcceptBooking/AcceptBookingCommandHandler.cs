using Application.Common.Interfaces;
using MediatR;
using System;

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

    public async Task<Unit> Handle(AcceptBookingCommand request, CancellationToken ct)
    {
        var booking = await _bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw new InvalidOperationException("Booking not found.");

        try
        {
            var dbRv = booking.RowVersion ?? Array.Empty<byte>();
            var reqRv = request.RowVersion ?? Array.Empty<byte>();
            Console.WriteLine($"[DEBUG] Booking RowVersion (db) length={dbRv.Length} base64={Convert.ToBase64String(dbRv)}");
            Console.WriteLine($"[DEBUG] Booking RowVersion (request) length={reqRv.Length} base64={Convert.ToBase64String(reqRv)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] Failed to log RowVersion values: {ex}");
        }

        if (!(booking.RowVersion ?? Array.Empty<byte>()).SequenceEqual(request.RowVersion ?? Array.Empty<byte>()))
            throw new InvalidOperationException("Booking version mismatch (concurrency error).");

        booking.Accept();
        await _bookings.UpdateAsync(booking, ct); 
        await _uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}