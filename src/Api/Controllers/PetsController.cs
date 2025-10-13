using Api.DTOs;
using Api.Mappings;
using Application.Pets.Commands.CreatePet;
using Application.Pets.Queries.GetPetById;
using Application.Pets.Queries.ListPetsForOwner;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PetsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePetDto request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreatePetCommand(
            request.OwnerId,
            request.Name,
            request.Type,
            request.Breed,
            request.Notes), ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var model = await _mediator.Send(new GetPetByIdQuery(id), ct);
        if (model is null) return NotFound();
        
        var petDto = new PetDto
        {
            Id = model.Id,
            OwnerId = model.OwnerId,
            Name = model.Name,
            Type = model.TypeString,
            Breed = model.Breed,
            Notes = model.Notes,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
        return Ok(petDto);
    }

    [HttpGet("owner/{ownerId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<PetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListForOwner(Guid ownerId, CancellationToken ct)
    {
        var pets = await _mediator.Send(new ListPetsForOwnerQuery(ownerId), ct);
        var petDtos = pets.Select(p => new PetDto
        {
            Id = p.Id,
            OwnerId = p.OwnerId,
            Name = p.Name,
            Type = p.TypeString,
            Breed = p.Breed,
            Notes = p.Notes,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });
        return Ok(petDtos);
    }
}
