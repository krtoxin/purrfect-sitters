using Application.Common.Interfaces;
using Application.Pets.Queries;
using Domain.Pets;
using MediatR;

namespace Infrastructure.Queries.Pets;

public class ListAllPetsQueryHandler : IRequestHandler<ListAllPetsQuery, IEnumerable<Pet>>
{
    private readonly IPetRepository _pets;
    public ListAllPetsQueryHandler(IPetRepository pets) => _pets = pets;

    public async Task<IEnumerable<Pet>> Handle(ListAllPetsQuery request, CancellationToken cancellationToken)
    {
        return await _pets.GetAllAsync(cancellationToken);
    }
}
