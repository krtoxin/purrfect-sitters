namespace Api.Contracts.Pets.Responses;

public record PetResponse(
    Guid Id,
    Guid OwnerId,
    string Name,
    string Type,
    string? Breed,
    string? Notes,
    DateTime CreatedAt
);
