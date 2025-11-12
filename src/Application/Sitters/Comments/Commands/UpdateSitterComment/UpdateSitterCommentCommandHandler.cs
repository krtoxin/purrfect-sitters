using Application.Common.Interfaces;
using MediatR;

namespace Application.Sitters.Comments.Commands.UpdateSitterComment;

public class UpdateSitterCommentCommandHandler : IRequestHandler<UpdateSitterCommentCommand, bool>
{
    private readonly ISitterCommentRepository _comments;
    private readonly IUnitOfWork _uow;
    public UpdateSitterCommentCommandHandler(ISitterCommentRepository comments, IUnitOfWork uow)
    {
        _comments = comments;
        _uow = uow;
    }

    public async Task<bool> Handle(UpdateSitterCommentCommand request, CancellationToken ct)
    {
        var comment = await _comments.GetByIdAsync(request.Id, ct);
        if (comment is null) return false;
        comment.UpdateContent(request.Content);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}