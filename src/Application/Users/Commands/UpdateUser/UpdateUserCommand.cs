using MediatR;

namespace Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string Name,
    bool IsActive
) : IRequest<bool>;
