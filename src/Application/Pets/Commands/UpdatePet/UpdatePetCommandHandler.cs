using Application.Common.Interfaces;
using MediatR;

namespace Application.Pets.Commands.UpdatePet;

public class UpdatePetCommandHandler : IRequestHandler<UpdatePetCommand, bool>
{
    private readonly IPetRepository _pets;
    private readonly IUnitOfWork _uow;
    public UpdatePetCommandHandler(IPetRepository pets, IUnitOfWork uow)
    {
        _pets = pets;
        _uow = uow;
    }

    public async Task<bool> Handle(UpdatePetCommand request, CancellationToken cancellationToken)
    {
        var pet = await _pets.GetByIdAsync(request.Id, cancellationToken);
        if (pet is null) return false;
        pet.Update(request.Name, request.Type, request.Breed, request.Notes);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
