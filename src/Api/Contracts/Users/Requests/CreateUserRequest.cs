namespace Api.Contracts.Users.Requests;

public record CreateUserRequest(
    string Email,
    string Name,
    int Roles = 1 // Default to Owner role
);
