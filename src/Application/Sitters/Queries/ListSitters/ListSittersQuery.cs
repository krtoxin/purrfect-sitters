using Application.Common.Models;
using Application.Sitters.Models;
using MediatR;

namespace Application.Sitters.Queries.ListSitters;

public record ListSittersQuery(int Page, int PageSize) : IRequest<PagedResult<SitterProfileReadModel>>;
