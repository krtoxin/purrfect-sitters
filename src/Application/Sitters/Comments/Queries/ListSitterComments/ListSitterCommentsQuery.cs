using Application.Sitters.Comments.Queries.Models;
using MediatR;

namespace Application.Sitters.Comments.Queries.ListSitterComments;

public sealed record ListSitterCommentsQuery(Guid SitterProfileId) : IRequest<IEnumerable<SitterCommentReadModel>>;