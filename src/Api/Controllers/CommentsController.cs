using Api.DTOs;
using Application.Sitters.Comments.Commands.UpdateSitterComment;
using Application.Sitters.Comments.Commands.DeleteSitterComment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;
    public CommentsController(IMediator mediator) => _mediator = mediator;

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSitterCommentDto body, CancellationToken ct)
    {
        var ok = await _mediator.Send(new UpdateSitterCommentCommand(id, body.Content), ct);
        if (!ok) return NotFound();
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DeleteSitterCommentCommand(id), ct);
        if (!ok) return NotFound();
        return NoContent();
    }
}