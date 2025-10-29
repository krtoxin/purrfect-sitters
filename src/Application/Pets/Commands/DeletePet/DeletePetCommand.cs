using MediatR;

namespace Application.Pets.Commands.DeletePet;

public sealed record DeletePetCommand(
    Guid Id
) : IRequest<bool>;
