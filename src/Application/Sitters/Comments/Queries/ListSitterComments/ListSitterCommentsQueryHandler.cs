using Application.Common.Interfaces;
using Application.Sitters.Comments.Queries.Models;
using MediatR;

namespace Application.Sitters.Comments.Queries.ListSitterComments;

public class ListSitterCommentsQueryHandler : IRequestHandler<ListSitterCommentsQuery, IEnumerable<SitterCommentReadModel>>
{
    private readonly ISitterCommentRepository _comments;
    private readonly ISitterProfileRepository _profiles;
    public ListSitterCommentsQueryHandler(ISitterCommentRepository comments, ISitterProfileRepository profiles)
    {
        _comments = comments;
        _profiles = profiles;
    }

    public async Task<IEnumerable<SitterCommentReadModel>> Handle(ListSitterCommentsQuery request, CancellationToken ct)
    {
        var sitter = await _profiles.GetByIdAsync(request.SitterProfileId, ct);
        if (sitter is null) throw new InvalidOperationException("Sitter profile not found.");
        var list = await _comments.ListForSitterAsync(request.SitterProfileId, ct);
        return list.Select(c => new SitterCommentReadModel(c.Id, c.SitterProfileId, c.Content, c.CreatedAt, c.UpdatedAt));
    }
}