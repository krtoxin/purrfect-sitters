using MediatR;

namespace Application.Pets.Commands.UpdatePet;

public sealed record UpdatePetCommand(
    Guid Id,
    string Name,
    string Type,
    string Breed,
    string? Notes
) : IRequest<bool>;
