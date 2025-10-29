using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Bookings.Commands.UpdateBooking;

public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand, bool>
{
    private readonly IBookingRepository _bookings;
    private readonly IUnitOfWork _uow;
    public UpdateBookingCommandHandler(IBookingRepository bookings, IUnitOfWork uow)
    {
        _bookings = bookings;
        _uow = uow;
    }

    public async Task<bool> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdAsync(request.Id, cancellationToken);
        if (booking is null) return false;

        booking.Update(request.StartUtc, request.EndUtc, request.BaseAmount, request.ServiceFeePercent, request.Currency, request.CareInstructionTexts);
        await _bookings.UpdateAsync(booking, cancellationToken);

        try
        {
            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            var fresh = await _bookings.GetByIdAsync(request.Id, cancellationToken);
            if (fresh is null) return false;

            fresh.Update(request.StartUtc, request.EndUtc, request.BaseAmount, request.ServiceFeePercent, request.Currency, request.CareInstructionTexts);
            await _bookings.UpdateAsync(fresh, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}