using MediatR;

namespace Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string Name,
    int Roles
) : IRequest<Guid>;
