using Application.Common.Interfaces;
using MediatR;

namespace Application.Bookings.Commands.DeleteBooking;

public class DeleteBookingCommandHandler : IRequestHandler<DeleteBookingCommand, bool>
{
    private readonly IBookingRepository _bookings;
    private readonly IUnitOfWork _uow;
    public DeleteBookingCommandHandler(IBookingRepository bookings, IUnitOfWork uow)
    {
        _bookings = bookings;
        _uow = uow;
    }

    public async Task<bool> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdAsync(request.Id, cancellationToken);
        if (booking is null) return false;
        await _bookings.DeleteAsync(booking, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
