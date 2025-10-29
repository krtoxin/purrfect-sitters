using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Bookings;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

        booking.Accept();
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
        
            if (fresh.Status == BookingStatus.Accepted)
                return Unit.Value; 
            fresh.Accept();
            await _bookings.UpdateAsync(fresh, ct);
            await _uow.SaveChangesAsync(ct); 
            return Unit.Value;
        }
    }
}