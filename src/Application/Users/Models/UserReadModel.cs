using Domain.Users;

namespace Application.Users.Models;

public record UserReadModel(
    Guid Id,
    string Email,
    string Name,
    IEnumerable<UserRole> Roles,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public bool IsActive { get; init; } = true;
    public int RolesInt => (int)Roles.FirstOrDefault();
}
