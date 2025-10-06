using Application.Common.Interfaces;
using Domain.Sitters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class SitterProfileRepository : ISitterProfileRepository
{
    private readonly ApplicationDbContext _db;
    public SitterProfileRepository(ApplicationDbContext db) => _db = db;

    public Task<SitterProfile?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.SitterProfiles.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task AddAsync(SitterProfile sitter, CancellationToken ct = default) =>
        await _db.SitterProfiles.AddAsync(sitter, ct);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        _db.SitterProfiles.AnyAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<SitterProfile>> GetAllAsync(int page, int pageSize, CancellationToken ct = default) =>
        await _db.SitterProfiles
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
}