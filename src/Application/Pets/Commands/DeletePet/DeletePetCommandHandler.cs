using Application.Common.Interfaces;
using MediatR;

namespace Application.Pets.Commands.DeletePet;

public class DeletePetCommandHandler : IRequestHandler<DeletePetCommand, bool>
{
    private readonly IPetRepository _pets;
    private readonly IUnitOfWork _uow;
    public DeletePetCommandHandler(IPetRepository pets, IUnitOfWork uow)
    {
        _pets = pets;
        _uow = uow;
    }

    public async Task<bool> Handle(DeletePetCommand request, CancellationToken cancellationToken)
    {
        var pet = await _pets.GetByIdAsync(request.Id, cancellationToken);
        if (pet is null) return false;
        await _pets.DeleteAsync(pet, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
