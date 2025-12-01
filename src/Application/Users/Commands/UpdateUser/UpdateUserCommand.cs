using MediatR;

namespace Application.Users.Commands.UpdateUser;

using Domain.Common;

public sealed record UpdateUserCommand(
    Guid Id,
    string Name,
    bool IsActive
) : IRequest<Result>;
