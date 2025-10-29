using MediatR;

namespace Application.Pets.Commands.CreatePet;

public record CreatePetCommand(
    Guid OwnerId,
    string Name,
    string Type,
    string? Breed,
    string? Notes) : IRequest<Guid>;
