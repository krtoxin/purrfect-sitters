using Application.Common.Interfaces;
using Domain.Sitters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SitterCommentRepository : ISitterCommentRepository
{
    private readonly ApplicationDbContext _context;
    public SitterCommentRepository(ApplicationDbContext context) => _context = context;

    public async Task<SitterComment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.SitterComments.FindAsync([id], ct);

    public async Task AddAsync(SitterComment comment, CancellationToken ct = default)
        => await _context.SitterComments.AddAsync(comment, ct);

    public async Task DeleteAsync(SitterComment comment, CancellationToken ct = default)
    {
        _context.SitterComments.Remove(comment);
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<SitterComment>> ListForSitterAsync(Guid sitterProfileId, CancellationToken ct = default)
        => await _context.SitterComments
            .AsNoTracking()
            .Where(c => c.SitterProfileId == sitterProfileId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);
}