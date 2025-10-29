using Application.Common.Interfaces;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    public UpdateUserCommandHandler(IUserRepository users, IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.Id, cancellationToken);
        if (user is null) return false;
        user.Rename(request.Name);
        if (user.IsActive != request.IsActive)
        {
            if (request.IsActive) user.Activate();
            else user.Deactivate();
        }
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
