using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _db;
    public BookingRepository(ApplicationDbContext db) => _db = db;

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Bookings
            .Include(b => b.StatusHistory)
            .Include(b => b.CareInstructionSnapshots)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task AddAsync(Booking booking, CancellationToken ct = default)
    {
    await _db.Bookings.AddAsync(booking, ct);
    await _db.SaveChangesAsync(ct); // Save to get xmin
    await _db.Entry(booking).ReloadAsync(ct); // Hydrate xmin
    var xmin = _db.Entry(booking).Property("xmin").CurrentValue;
    Console.WriteLine($"[REPO] Booking added, saved, and reloaded. xmin: {xmin}");
    }

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

    public async Task<IReadOnlyList<Booking>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Bookings.AsNoTracking().ToListAsync(ct);

    public async Task UpdateAsync(Booking booking, CancellationToken ct = default)
    {
        var entry = _db.Entry(booking);
        var currentStatus = booking.Status;
        
        if (entry.State != EntityState.Detached)
        {
            var statusProp = entry.Property("Status");
            var originalStatus = statusProp != null ? (BookingStatus)statusProp.OriginalValue! : currentStatus;
            
            if (originalStatus != currentStatus)
            {
                var statusInt = (int)currentStatus;
                await _db.Database.ExecuteSqlRawAsync(
                    "UPDATE bookings SET status = {0}, updated_at = timezone('utc', now()) WHERE id = {1}",
                    statusInt, booking.Id);
                
                entry.State = EntityState.Detached;
                _db.Bookings.Update(booking);
                
                var newEntry = _db.Entry(booking);
                newEntry.State = EntityState.Modified;
                var newStatusProp = newEntry.Property("Status");
                if (newStatusProp != null)
                {
                    newStatusProp.IsModified = false;
                }
            }
            else
            {
                entry.State = EntityState.Detached;
                _db.Bookings.Update(booking);
            }
        }
        else
        {
            _db.Bookings.Update(booking);
        }
        
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Booking booking, CancellationToken ct = default)
    {
        _db.Bookings.Remove(booking);
        await Task.CompletedTask;
    }
}