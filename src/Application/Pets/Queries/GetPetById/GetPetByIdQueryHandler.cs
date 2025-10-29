using Application.Common.Interfaces;
using Application.Pets.Models;
using MediatR;

namespace Application.Pets.Queries.GetPetById;

public class GetPetByIdQueryHandler : IRequestHandler<GetPetByIdQuery, PetReadModel?>
{
    private readonly IPetRepository _petRepository;

    public GetPetByIdQueryHandler(IPetRepository petRepository)
    {
        _petRepository = petRepository;
    }

    public async Task<PetReadModel?> Handle(GetPetByIdQuery request, CancellationToken cancellationToken)
    {
        var pet = await _petRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (pet is null)
            return null;

        return new PetReadModel(
            pet.Id,
            pet.OwnerId,
            pet.Name,
            pet.Type,
            pet.Breed,
            pet.Notes,
            pet.CreatedAt,
            pet.CreatedAt);
    }
}
