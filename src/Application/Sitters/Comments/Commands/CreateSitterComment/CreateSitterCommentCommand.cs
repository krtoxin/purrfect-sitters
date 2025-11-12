using MediatR;

namespace Application.Sitters.Comments.Commands.CreateSitterComment;

public sealed record CreateSitterCommentCommand(Guid SitterProfileId, string Content) : IRequest<Guid>;