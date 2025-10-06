using Application.Bookings.Models;
using Application.Common.Models;
using MediatR;

namespace Application.Bookings.Queries.ListBookingsForOwner;

public sealed record ListBookingsForOwnerQuery(Guid OwnerId, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<BookingReadModel>>;