using Application.Common.Interfaces;
using Domain.Users;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Users.FindAsync([id], ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _context.Users.AddAsync(user, ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await _context.Users.AnyAsync(u => u.Id == id, ct);

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
        => await _context.Users.AsNoTracking().ToListAsync(ct);

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Update(user);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Remove(user);
        await Task.CompletedTask;
    }
}
