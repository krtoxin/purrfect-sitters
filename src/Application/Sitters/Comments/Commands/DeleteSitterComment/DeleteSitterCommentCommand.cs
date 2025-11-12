using MediatR;

namespace Application.Sitters.Comments.Commands.DeleteSitterComment;

public sealed record DeleteSitterCommentCommand(Guid Id) : IRequest<bool>;