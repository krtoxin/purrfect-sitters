using Domain.Sitters;

namespace Application.Common.Interfaces;

public interface ISitterRepository
{
    Task<Sitter?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Sitter sitter, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Sitter>> GetAllAsync(CancellationToken ct = default);
    Task UpdateAsync(Sitter sitter, CancellationToken ct = default);
    Task DeleteAsync(Sitter sitter, CancellationToken ct = default);
}
