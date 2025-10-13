using Application.Sitters.Models;
using MediatR;

namespace Application.Sitters.Queries.GetSitterById;

public record GetSitterByIdQuery(Guid Id) : IRequest<SitterProfileReadModel?>;
