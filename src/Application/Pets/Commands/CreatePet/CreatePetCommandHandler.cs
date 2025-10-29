using Application.Common.Interfaces;
using Application.Pets.Models;
using Domain.Pets;
using MediatR;

namespace Application.Pets.Commands.CreatePet;

public class CreatePetCommandHandler : IRequestHandler<CreatePetCommand, Guid>
{
    private readonly IPetRepository _petRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePetCommandHandler(IPetRepository petRepository, IUnitOfWork unitOfWork)
    {
        _petRepository = petRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreatePetCommand request, CancellationToken cancellationToken)
    {
        var petType = Enum.Parse<PetType>(request.Type, ignoreCase: true);
        
        var pet = Pet.Create(
            Guid.NewGuid(),
            request.OwnerId,
            request.Name,
            petType,
            request.Breed,
            request.Notes);

        await _petRepository.AddAsync(pet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return pet.Id;
    }
}
