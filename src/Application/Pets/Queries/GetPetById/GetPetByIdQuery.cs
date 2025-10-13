using Application.Pets.Models;
using MediatR;

namespace Application.Pets.Queries.GetPetById;

public record GetPetByIdQuery(Guid Id) : IRequest<PetReadModel?>;
