using MediatR;
using Domain.Bookings;

namespace Application.Bookings.Queries;

public class ListAllBookingsQuery : IRequest<IEnumerable<Booking>>
{
}
