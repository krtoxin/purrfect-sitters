using Application.Common.Interfaces;
using Domain.Bookings;
using MediatR;

namespace Application.Bookings.Queries;

public class ListAllBookingsQueryHandler : IRequestHandler<ListAllBookingsQuery, IEnumerable<Booking>>
{
    private readonly IBookingRepository _bookings;
    public ListAllBookingsQueryHandler(IBookingRepository bookings) => _bookings = bookings;

    public async Task<IEnumerable<Booking>> Handle(ListAllBookingsQuery request, CancellationToken cancellationToken)
    {
        return await _bookings.GetAllAsync(cancellationToken);
    }
}
