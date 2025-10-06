using Application.Bookings.Models;
using MediatR;

namespace Application.Bookings.Queries.GetBookingById;

public sealed record GetBookingByIdQuery(Guid Id) : IRequest<BookingReadModel?>;