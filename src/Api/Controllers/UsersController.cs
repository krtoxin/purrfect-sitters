using Api.DTOs;
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
    public async Task<IActionResult> Create([FromBody] CreateUserDto request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateUserCommand(
            request.Email,
            request.Name,
            (int)Enum.Parse<Domain.Users.UserRole>(request.Roles, ignoreCase: true)), ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var model = await _mediator.Send(new GetUserByIdQuery(id), ct);
        if (model is null) return NotFound();
        var userDto = new UserDto
        {
            Id = model.Id,
            Email = model.Email,
            Name = model.Name,
            Roles = string.Join(",", model.Roles.Select(r => r.ToString())),
            IsActive = model.IsActive,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        };
        return Ok(userDto);
    }
}
