using Domain.Pets;

namespace Application.Common.Interfaces;
public interface IPetRepository
{
    Task<Pet?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Pet pet, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Pet>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Pet>> GetForOwnerAsync(Guid ownerId, CancellationToken ct = default);
}