using Api.Contracts.Users.Requests;
using Api.Contracts.Users.Responses;
using Api.Mappings;
using Application.Users.Commands.CreateUser;
using Application.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateUserCommand(
            request.Email,
            request.Name,
            request.Roles), ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var model = await _mediator.Send(new GetUserByIdQuery(id), ct);
        if (model is null) return NotFound();
        return Ok(model.ToResponse());
    }
}
