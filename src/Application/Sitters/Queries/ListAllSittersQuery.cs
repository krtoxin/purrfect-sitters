using MediatR;
using Domain.Sitters;

namespace Application.Sitters.Queries;

public class ListAllSittersQuery : IRequest<IEnumerable<SitterProfile>>
{
}
