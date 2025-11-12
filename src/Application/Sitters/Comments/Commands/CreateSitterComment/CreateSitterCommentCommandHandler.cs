using Application.Common.Interfaces;
using Domain.Sitters;
using MediatR;

namespace Application.Sitters.Comments.Commands.CreateSitterComment;

public class CreateSitterCommentCommandHandler : IRequestHandler<CreateSitterCommentCommand, Guid>
{
    private readonly ISitterProfileRepository _profiles;
    private readonly ISitterCommentRepository _comments;
    private readonly IUnitOfWork _uow;

    public CreateSitterCommentCommandHandler(ISitterProfileRepository profiles, ISitterCommentRepository comments, IUnitOfWork uow)
    {
        _profiles = profiles;
        _comments = comments;
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateSitterCommentCommand request, CancellationToken ct)
    {
        var sitter = await _profiles.GetByIdAsync(request.SitterProfileId, ct);
        if (sitter is null) throw new InvalidOperationException("Sitter profile not found.");

        var comment = SitterComment.Create(request.SitterProfileId, request.Content);
        await _comments.AddAsync(comment, ct);
        await _uow.SaveChangesAsync(ct);
        return comment.Id;
    }
}