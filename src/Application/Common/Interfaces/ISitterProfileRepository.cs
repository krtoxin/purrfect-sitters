using Domain.Sitters;

namespace Application.Common.Interfaces;
public interface ISitterProfileRepository
{
    Task<SitterProfile?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(SitterProfile sitter, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<SitterProfile>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
    Task<IReadOnlyList<SitterProfile>> GetAllAsync(CancellationToken ct = default);
}