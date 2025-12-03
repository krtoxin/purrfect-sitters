using MediatR;

namespace Application.Sitters.Comments.Commands.UpdateSitterComment;

public sealed record UpdateSitterCommentCommand(Guid Id, string Content) : IRequest<bool>;