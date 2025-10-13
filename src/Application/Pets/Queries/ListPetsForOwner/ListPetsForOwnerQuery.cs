using Application.Pets.Models;
using MediatR;

namespace Application.Pets.Queries.ListPetsForOwner;

public record ListPetsForOwnerQuery(Guid OwnerId) : IRequest<IEnumerable<PetReadModel>>;
