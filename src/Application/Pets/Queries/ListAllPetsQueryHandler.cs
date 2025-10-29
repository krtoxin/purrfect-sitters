using Application.Common.Interfaces;
using Domain.Pets;
using MediatR;

namespace Application.Pets.Queries;

public class ListAllPetsQueryHandler : IRequestHandler<ListAllPetsQuery, IEnumerable<Pet>>
{
    private readonly IPetRepository _pets;
    public ListAllPetsQueryHandler(IPetRepository pets) => _pets = pets;

    public async Task<IEnumerable<Pet>> Handle(ListAllPetsQuery request, CancellationToken cancellationToken)
    {
        return await _pets.GetAllAsync(cancellationToken);
    }
}