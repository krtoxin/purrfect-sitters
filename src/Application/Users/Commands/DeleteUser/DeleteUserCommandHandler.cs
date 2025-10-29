using Application.Common.Interfaces;
using MediatR;

namespace Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    public DeleteUserCommandHandler(IUserRepository users, IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.Id, cancellationToken);
        if (user is null) return false;
        await _users.DeleteAsync(user, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
