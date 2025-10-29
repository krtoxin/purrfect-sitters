using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Bookings;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

        booking.Complete();
        await _bookings.UpdateAsync(booking, ct);

        try
        {
            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
        catch (DbUpdateConcurrencyException)
        {
            var fresh = await _bookings.GetByIdAsync(request.BookingId, ct)
                ?? throw new InvalidOperationException("Booking not found after concurrency failure.");

            if (fresh.Status == BookingStatus.Completed)
                return Unit.Value;

            fresh.Complete();
            await _bookings.UpdateAsync(fresh, ct);
            await _uow.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}