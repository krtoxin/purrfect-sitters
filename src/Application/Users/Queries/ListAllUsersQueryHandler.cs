using Application.Common.Interfaces;
using Domain.Users;
using MediatR;

namespace Application.Users.Queries;

public class ListAllUsersQueryHandler : IRequestHandler<ListAllUsersQuery, IEnumerable<User>>
{
    private readonly IUserRepository _users;
    public ListAllUsersQueryHandler(IUserRepository users) => _users = users;

    public async Task<IEnumerable<User>> Handle(ListAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await _users.GetAllAsync(cancellationToken);
    }
}