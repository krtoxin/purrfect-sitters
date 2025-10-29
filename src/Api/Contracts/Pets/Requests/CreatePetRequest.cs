namespace Api.Contracts.Pets.Requests;

public record CreatePetRequest(
    Guid OwnerId,
    string Name,
    string Type,
    string? Breed = null,
    string? Notes = null
);
