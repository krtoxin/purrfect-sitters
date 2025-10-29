using Application.Common.Interfaces;
using Application.Pets.Models;
using MediatR;

namespace Application.Pets.Queries.ListPetsForOwner;

public class ListPetsForOwnerQueryHandler : IRequestHandler<ListPetsForOwnerQuery, IEnumerable<PetReadModel>>
{
    private readonly IPetRepository _petRepository;

    public ListPetsForOwnerQueryHandler(IPetRepository petRepository)
    {
        _petRepository = petRepository;
    }

    public async Task<IEnumerable<PetReadModel>> Handle(ListPetsForOwnerQuery request, CancellationToken cancellationToken)
    {
        var pets = await _petRepository.GetForOwnerAsync(request.OwnerId, cancellationToken);
        
        return pets.Select(pet => new PetReadModel(
            pet.Id,
            pet.OwnerId,
            pet.Name,
            pet.Type,
            pet.Breed,
            pet.Notes,
            pet.CreatedAt,
            pet.CreatedAt));
    }
}
