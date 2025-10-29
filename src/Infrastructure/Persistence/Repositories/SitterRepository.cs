using Application.Common.Interfaces;
using Domain.Sitters;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SitterRepository : ISitterRepository
{
    private readonly ApplicationDbContext _context;

    public SitterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Sitter?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Sitters.FindAsync([id], ct);

    public async Task AddAsync(Sitter sitter, CancellationToken ct = default)
    {
        await _context.Sitters.AddAsync(sitter, ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _context.Sitters.AnyAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<Sitter>> GetAllAsync(CancellationToken ct = default)
        => await _context.Sitters.AsNoTracking().ToListAsync(ct);

    public async Task UpdateAsync(Sitter sitter, CancellationToken ct = default)
    {
        _context.Sitters.Update(sitter);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Sitter sitter, CancellationToken ct = default)
    {
        _context.Sitters.Remove(sitter);
        await Task.CompletedTask;
    }
}
