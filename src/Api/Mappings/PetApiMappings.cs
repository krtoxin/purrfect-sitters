using Api.Contracts.Pets.Responses;
using Application.Pets.Models;

namespace Api.Mappings;

public static class PetApiMappings
{
    public static PetResponse ToResponse(this PetReadModel model)
        => new(model.Id, model.OwnerId, model.Name, model.Type, model.Breed, model.Notes, model.CreatedAt);
}
