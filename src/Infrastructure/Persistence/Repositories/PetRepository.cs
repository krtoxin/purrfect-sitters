using Application.Common.Interfaces;
using Domain.Pets;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class PetRepository : IPetRepository
{
    private readonly ApplicationDbContext _db;
    public PetRepository(ApplicationDbContext db) => _db = db;

    public Task<Pet?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Pets.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task AddAsync(Pet pet, CancellationToken ct = default) =>
        await _db.Pets.AddAsync(pet, ct);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        _db.Pets.AnyAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Pet>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Pets.AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<Pet>> GetForOwnerAsync(Guid ownerId, CancellationToken ct = default) =>
        await _db.Pets
            .Where(p => p.OwnerId == ownerId)
            .ToListAsync(ct);

    public async Task UpdateAsync(Pet pet, CancellationToken ct = default)
    {
        _db.Pets.Update(pet);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Pet pet, CancellationToken ct = default)
    {
        _db.Pets.Remove(pet);
        await Task.CompletedTask;
    }
}