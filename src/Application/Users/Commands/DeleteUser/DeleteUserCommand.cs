using MediatR;

namespace Application.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(
    Guid Id
) : IRequest<bool>;
