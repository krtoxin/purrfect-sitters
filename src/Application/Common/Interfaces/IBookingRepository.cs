using Domain.Bookings;

namespace Application.Common.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Booking booking, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken ct = default);

    Task<IReadOnlyList<Booking>> GetForOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken ct = default);

    Task<(IReadOnlyList<Booking> Items, long TotalCount)> GetForOwnerPagedAsync(
        Guid ownerId,
        int page,
        int pageSize,
        CancellationToken ct = default);
}