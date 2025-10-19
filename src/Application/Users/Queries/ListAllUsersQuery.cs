using MediatR;
using Domain.Users;

namespace Application.Users.Queries;

public class ListAllUsersQuery : IRequest<IEnumerable<User>>
{
}
