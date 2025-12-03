using Api.DTOs;
using Application.Sitters.Comments.Commands.CreateSitterComment;
using Application.Sitters.Comments.Queries.ListSitterComments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/sitters/{sitterId:guid}/comments")]
public class SitterCommentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public SitterCommentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SitterCommentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(Guid sitterId, CancellationToken ct)
    {
        var items = await _mediator.Send(new ListSitterCommentsQuery(sitterId), ct);
        var dtos = items.Select(c => new SitterCommentDto
        {
            Id = c.Id,
            SitterProfileId = c.SitterProfileId,
            Content = c.Content,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        });
        return Ok(dtos);
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(Guid sitterId, [FromBody] CreateSitterCommentDto body, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateSitterCommentCommand(sitterId, body.Content), ct);
        return CreatedAtAction(nameof(List), new { sitterId }, new { id });
    }
}