using Api.DTOs;
using Api.Mappings;
using Application.Sitters.Commands.CreateSitterProfile;
using Application.Sitters.Queries.GetSitterById;
using Application.Sitters.Queries.ListSitters;
using Application.Sitters.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SittersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SittersController(IMediator mediator) => _mediator = mediator;


    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSitterDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new Application.Sitters.Commands.UpdateSitter.UpdateSitterCommand(
            id,
            request.Bio,
            request.BaseRateAmount ?? 0,
            request.BaseRateCurrency ?? "USD",
            request.ServicesOffered.Split(',', StringSplitOptions.RemoveEmptyEntries)), ct);
        if (!result)
            return NotFound();
        return Ok();
    }


    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new Application.Sitters.Commands.DeleteSitter.DeleteSitterCommand(id), ct);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSitterDto request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateSitterProfileCommand(
            request.UserId,
            request.Bio,
            request.BaseRateAmount ?? 0,
            request.BaseRateCurrency ?? "USD",
            new[] { request.ServicesOffered }), ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SitterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var model = await _mediator.Send(new GetSitterByIdQuery(id), ct);
        if (model is null) return NotFound();
        
        var sitterDto = new SitterDto
        {
            Id = model.Id,
            UserId = model.UserId,
            Bio = model.Bio,
            IsActive = model.IsActive,
            AverageRating = model.AverageRating,
            BaseRateAmount = model.BaseRateAmount,
            BaseRateCurrency = model.BaseRateCurrency,
            ServicesOffered = string.Join(",", model.ServicesOffered.Select(s => s.ToString())),
            CompletedBookings = model.CompletedBookings,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
        return Ok(sitterDto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SitterDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var sitters = await _mediator.Send(new ListSittersQuery(page, pageSize), ct);
        var sitterDtos = sitters.Items.Select(s => new SitterDto
        {
            Id = s.Id,
            UserId = s.UserId,
            Bio = s.Bio,
            IsActive = s.IsActive,
            AverageRating = s.AverageRating,
            BaseRateAmount = s.BaseRateAmount,
            BaseRateCurrency = s.BaseRateCurrency,
            ServicesOffered = string.Join(",", s.ServicesOffered.Select(sv => sv.ToString())),
            CompletedBookings = s.CompletedBookings,
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        });
        return Ok(sitterDtos);
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<SitterDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var sitters = await _mediator.Send(new ListAllSittersQuery(), ct);
        var sitterDtos = sitters.Select(s => new SitterDto
        {
            Id = s.Id,
            UserId = s.UserId,
            Bio = s.Bio,
            IsActive = s.IsActive,
            AverageRating = s.AverageRating,
            BaseRateAmount = s.BaseRate?.Amount,
            BaseRateCurrency = s.BaseRate?.Currency,
            ServicesOffered = s.ServicesOffered.ToString(),
            CompletedBookings = s.CompletedBookings,
            CreatedAt = s.CreatedAt
        });
        return Ok(sitterDtos);
    }
}
