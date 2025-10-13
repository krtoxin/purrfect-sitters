using Api.Contracts.Sitters.Requests;
using Api.Contracts.Sitters.Responses;
using Api.Mappings;
using Application.Sitters.Commands.CreateSitterProfile;
using Application.Sitters.Queries.GetSitterById;
using Application.Sitters.Queries.ListSitters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SittersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SittersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSitterProfileRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateSitterProfileCommand(
            request.UserId,
            request.Bio,
            request.BaseRateAmount,
            request.BaseRateCurrency,
            request.ServicesOffered), ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SitterProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var model = await _mediator.Send(new GetSitterByIdQuery(id), ct);
        if (model is null) return NotFound();
        return Ok(model.ToResponse());
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SitterProfileResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var sitters = await _mediator.Send(new ListSittersQuery(page, pageSize), ct);
        return Ok(sitters.Select(s => s.ToResponse()));
    }
}
