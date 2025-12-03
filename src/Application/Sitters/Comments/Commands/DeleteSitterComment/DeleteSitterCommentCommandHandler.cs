using Application.Common.Interfaces;
using MediatR;

namespace Application.Sitters.Comments.Commands.DeleteSitterComment;

public class DeleteSitterCommentCommandHandler : IRequestHandler<DeleteSitterCommentCommand, bool>
{
    private readonly ISitterCommentRepository _comments;
    private readonly IUnitOfWork _uow;
    public DeleteSitterCommentCommandHandler(ISitterCommentRepository comments, IUnitOfWork uow)
    {
        _comments = comments;
        _uow = uow;
    }

    public async Task<bool> Handle(DeleteSitterCommentCommand request, CancellationToken ct)
    {
        var comment = await _comments.GetByIdAsync(request.Id, ct);
        if (comment is null) return false;
        await _comments.DeleteAsync(comment, ct);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}