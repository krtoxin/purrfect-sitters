using Domain.Sitters;

namespace Application.Common.Interfaces;

public interface ISitterCommentRepository
{
    Task<SitterComment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(SitterComment comment, CancellationToken ct = default);
    Task DeleteAsync(SitterComment comment, CancellationToken ct = default);
    Task<IReadOnlyList<SitterComment>> ListForSitterAsync(Guid sitterProfileId, CancellationToken ct = default);
}