using Application.Common.Interfaces;
using Application.Users.Models;
using MediatR;

namespace Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserReadModel?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserReadModel?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (user is null)
            return null;

        return new UserReadModel(
            user.Id,
            user.Email.Value,
            user.Name,
            new[] { user.Roles },
            user.CreatedAt,
            user.UpdatedAt ?? user.CreatedAt);
    }
}
