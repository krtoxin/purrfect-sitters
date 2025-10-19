using Application.Common.Interfaces;
using Domain.Sitters;
using MediatR;

namespace Application.Sitters.Queries;

public class ListAllSittersQueryHandler : IRequestHandler<ListAllSittersQuery, IEnumerable<SitterProfile>>
{
    private readonly ISitterProfileRepository _sitters;
    public ListAllSittersQueryHandler(ISitterProfileRepository sitters) => _sitters = sitters;

    public async Task<IEnumerable<SitterProfile>> Handle(ListAllSittersQuery request, CancellationToken cancellationToken)
    {
        return await _sitters.GetAllAsync(cancellationToken);
    }
}
