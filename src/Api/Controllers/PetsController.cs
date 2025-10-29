using Api.DTOs;
using Api.Mappings;
using Application.Pets.Commands.CreatePet;
using Application.Pets.Commands.UpdatePet;
using Application.Pets.Commands.DeletePet;
using Application.Pets.Queries.GetPetById;
using Application.Pets.Queries.ListPetsForOwner;
using Application.Pets.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PetsController(IMediator mediator) => _mediator = mediator;

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePetDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdatePetCommand(
            id,
            request.Name,
            string.Empty, 
            request.Breed ?? string.Empty,
            request.Notes), ct);
        if (!result)
            return NotFound();
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeletePetCommand(id), ct);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePetDto request, CancellationToken ct)
    {
        // Robust validation for test: reject empty/whitespace OwnerId, Name, or Type
        if (request.OwnerId == Guid.Empty || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Type))
            return BadRequest();
        if (string.IsNullOrWhiteSpace(request.Name?.Trim()) || request.Name.Trim().Length < 2)
            return BadRequest();

        var id = await _mediator.Send(new CreatePetCommand(
            request.OwnerId,
            request.Name,
            request.Type,
            request.Breed,
            request.Notes), ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var pets = await _mediator.Send(new ListAllPetsQuery(), ct);
        var petDtos = pets.Select(p => new PetDto
        {
            Id = p.Id,
            OwnerId = p.OwnerId,
            Name = p.Name,
            Type = p.Type.ToString(),
            Breed = p.Breed,
            Notes = p.Notes,
            CreatedAt = p.CreatedAt
        });
        return Ok(petDtos);
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
            Type = model.Type.ToString(),
            Breed = model.Breed,
            Notes = model.Notes,
            CreatedAt = model.CreatedAt
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
            Type = p.Type.ToString(),
            Breed = p.Breed,
            Notes = p.Notes,
            CreatedAt = p.CreatedAt
        });
        return Ok(petDtos);
    }
}
