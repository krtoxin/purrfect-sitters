using Application.Common.Interfaces;
using Domain.Owners;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class OwnerProfileRepository : IOwnerProfileRepository
{
    private readonly ApplicationDbContext _db;
    public OwnerProfileRepository(ApplicationDbContext db) => _db = db;

    public Task<OwnerProfile?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.OwnerProfiles.FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task AddAsync(OwnerProfile owner, CancellationToken ct = default) =>
        await _db.OwnerProfiles.AddAsync(owner, ct);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        _db.OwnerProfiles.AnyAsync(o => o.Id == id, ct);
}