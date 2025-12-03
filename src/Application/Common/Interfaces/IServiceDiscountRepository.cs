using Domain.Sitters;

namespace Application.Common.Interfaces;

public interface IServiceDiscountRepository
{
    Task<ServiceDiscount?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(ServiceDiscount discount, CancellationToken ct = default);
    Task UpdateAsync(ServiceDiscount discount, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceDiscount>> ListActiveByCategoryAsync(SitterServiceType category, DateTime utcNow, CancellationToken ct = default);
    Task<IReadOnlyList<ServiceDiscount>> ListAllAsync(CancellationToken ct = default);
}
