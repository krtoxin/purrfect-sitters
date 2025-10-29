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
        // Load and track the entity
        var booking = await _bookings.GetByIdAsync(request.BookingId, ct)
            ?? throw new InvalidOperationException("Booking not found.");

    Console.WriteLine($"[ACCEPT] Start - BookingId={booking.Id} Status={booking.Status} (Tracked: {((_uow as dynamic)?.IsTracked(booking))})");

        // Apply domain transition
    booking.Accept();
    Console.WriteLine($"[ACCEPT] After Accept() - BookingId={booking.Id} Status={booking.Status} (Tracked: {((_uow as dynamic)?.IsTracked(booking))})");

        try
        {
            Console.WriteLine("[ACCEPT] Attempting SaveChanges...");
            await _uow.SaveChangesAsync(ct);
            Console.WriteLine("[ACCEPT] SaveChanges succeeded.");
            // Reload to hydrate xmin and confirm status
            var after = await _bookings.GetByIdAsync(request.BookingId, ct);
            Console.WriteLine($"[ACCEPT] After Save - BookingId={after?.Id} Status={after?.Status}");
            if (after?.Status != BookingStatus.Accepted)
            {
                Console.WriteLine($"[ACCEPT] ERROR: Status after save is not Accepted! Actual: {after?.Status}");
                throw new InvalidOperationException($"Booking status after save is not Accepted! Actual: {after?.Status}");
            }
            return Unit.Value;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Console.WriteLine($"[ACCEPT] DbUpdateConcurrencyException: {ex.Message}");
            // Reload and retry once
            var fresh = await _bookings.GetByIdAsync(request.BookingId, ct)
                ?? throw new InvalidOperationException("Booking not found after concurrency failure.");
            Console.WriteLine($"[ACCEPT] Fresh load after concurrency - BookingId={fresh.Id} Status={fresh.Status}");
            if (fresh.Status == BookingStatus.Accepted)
            {
                Console.WriteLine("[ACCEPT] Fresh status is Accepted - treating as success.");
                return Unit.Value;
            }
            fresh.Accept();
            await _uow.SaveChangesAsync(ct);
            var afterRetry = await _bookings.GetByIdAsync(request.BookingId, ct);
            Console.WriteLine($"[ACCEPT] After retry - BookingId={afterRetry?.Id} Status={afterRetry?.Status}");
            if (afterRetry?.Status != BookingStatus.Accepted)
            {
                Console.WriteLine($"[ACCEPT] ERROR: Status after retry is not Accepted! Actual: {afterRetry?.Status}");
                throw new InvalidOperationException($"Booking status after retry is not Accepted! Actual: {afterRetry?.Status}");
            }
            return Unit.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ACCEPT] Unexpected exception: {ex}");
            throw;
        }
    }
}