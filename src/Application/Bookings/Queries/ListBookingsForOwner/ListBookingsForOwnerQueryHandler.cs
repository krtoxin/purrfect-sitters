using Application.Bookings.Models;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;

namespace Application.Bookings.Queries.ListBookingsForOwner;

public class ListBookingsForOwnerQueryHandler :
    IRequestHandler<ListBookingsForOwnerQuery, PagedResult<BookingReadModel>>
{
    private readonly IBookingRepository _bookings;

    public ListBookingsForOwnerQueryHandler(IBookingRepository bookings)
    {
        _bookings = bookings;
    }

    public async Task<PagedResult<BookingReadModel>> Handle(
        ListBookingsForOwnerQuery request,
        CancellationToken ct)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var size = request.PageSize <= 0 ? 20 : request.PageSize;

        var (items, total) = await _bookings.GetForOwnerPagedAsync(
            request.OwnerId,
            page,
            size,
            ct);

        var projected = items.Select(b => b.ToReadModel()).ToList();
        return PagedResult<BookingReadModel>.Create(projected, page, size, total);
    }
}