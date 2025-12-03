namespace Application.Sitters.Comments.Queries.Models;

public record SitterCommentReadModel(
    Guid Id,
    Guid SitterProfileId,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt);