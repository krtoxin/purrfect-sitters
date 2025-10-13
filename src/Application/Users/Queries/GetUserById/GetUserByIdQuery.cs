using Application.Users.Models;
using MediatR;

namespace Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserReadModel?>;
