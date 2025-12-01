using Application.Common.Interfaces;
using Domain.Sitters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ServiceDiscountRepository : IServiceDiscountRepository
{
    private readonly ApplicationDbContext _context;

    public ServiceDiscountRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceDiscount?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.ServiceDiscounts.FindAsync([id], ct);
    }

    public async Task AddAsync(ServiceDiscount discount, CancellationToken ct = default)
    {
        await _context.ServiceDiscounts.AddAsync(discount, ct);
    }

    public Task UpdateAsync(ServiceDiscount discount, CancellationToken ct = default)
    {
        _context.ServiceDiscounts.Update(discount);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity != null)
        {
            _context.ServiceDiscounts.Remove(entity);
        }
    }

    public async Task<IReadOnlyList<ServiceDiscount>> ListActiveByCategoryAsync(SitterServiceType category, DateTime utcNow, CancellationToken ct = default)
    {
        return await _context.ServiceDiscounts
            .Where(d => d.Category == category && d.ExpiresAt > utcNow)
            .OrderByDescending(d => d.Percentage)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ServiceDiscount>> ListAllAsync(CancellationToken ct = default)
    {
        return await _context.ServiceDiscounts
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);
    }
}
