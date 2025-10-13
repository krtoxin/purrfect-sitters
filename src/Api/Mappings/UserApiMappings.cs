using Api.Contracts.Users.Responses;
using Application.Users.Models;

namespace Api.Mappings;

public static class UserApiMappings
{
    public static UserResponse ToResponse(this UserReadModel model)
        => new(model.Id, model.Email, model.Name, model.RolesInt, model.IsActive, model.CreatedAt);
}
