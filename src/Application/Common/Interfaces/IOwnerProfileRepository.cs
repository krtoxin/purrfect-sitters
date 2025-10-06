using Domain.Owners;

namespace Application.Common.Interfaces;
public interface IOwnerProfileRepository
{
    Task<OwnerProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(OwnerProfile owner, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}