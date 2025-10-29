using Application.Bookings.Models;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Bookings.Queries.GetBookingById;

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingReadModel?>
{
    private readonly IBookingRepository _bookings;

    public GetBookingByIdQueryHandler(IBookingRepository bookings)
    {
        _bookings = bookings;
    }

    public async Task<BookingReadModel?> Handle(GetBookingByIdQuery request, CancellationToken ct)
    {
        var booking = await _bookings.GetByIdAsync(request.Id, ct);
        return booking?.ToReadModel();
    }
}