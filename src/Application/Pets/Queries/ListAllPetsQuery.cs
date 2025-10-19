using MediatR;
using Domain.Pets;
using System.Collections.Generic;

namespace Application.Pets.Queries
{
    public class ListAllPetsQuery : IRequest<IEnumerable<Pet>>
    {
    }
}
