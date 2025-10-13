using Application.Common.Interfaces;
using Domain.Users;
using Domain.ValueObjects;
using MediatR;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public CreateUserCommandHandler(IUserRepository users, IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var email = Email.Create(request.Email);
        var roles = (UserRole)request.Roles;
        
        var user = User.Register(Guid.NewGuid(), email, request.Name, roles);
        
        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
        
        return user.Id;
    }
}
