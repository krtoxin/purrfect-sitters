namespace Api.Contracts.Users.Responses;

public record UserResponse(
    Guid Id,
    string Email,
    string Name,
    int Roles,
    bool IsActive,
    DateTime CreatedAt
);
