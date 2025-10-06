using Application.Common.Interfaces;
using Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _db;
    public BookingRepository(ApplicationDbContext db) => _db = db;

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Bookings.FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task AddAsync(Booking booking, CancellationToken ct = default) =>
        await _db.Bookings.AddAsync(booking, ct);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        _db.Bookings.AnyAsync(b => b.Id == id, ct);

    public async Task<IReadOnlyList<Booking>> GetForOwnerAsync(Guid ownerId, int page, int pageSize, CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        return await _db.Bookings
            .Where(b => b.OwnerId == ownerId)
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<(IReadOnlyList<Booking> Items, long TotalCount)> GetForOwnerPagedAsync(
        Guid ownerId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var baseQuery = _db.Bookings
            .Where(b => b.OwnerId == ownerId);

        var total = await baseQuery.LongCountAsync(ct);

        var items = await baseQuery
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(b => b.StatusHistory)
            .Include(b => b.CareInstructionSnapshots)
            .AsNoTracking()
            .ToListAsync(ct);

        return (items, total);
    }
}