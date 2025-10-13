using Domain.Pets;

namespace Application.Pets.Models;

public record PetReadModel(
    Guid Id,
    Guid OwnerId,
    string Name,
    PetType Type,
    string? Breed,
    string? Notes,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public string TypeString => Type.ToString();
}
