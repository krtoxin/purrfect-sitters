using Api.DTOs;
using Api.Mappings;
using Application.Users.Commands.CreateUser;
using Application.Users.Queries.GetUserById;
using Application.Users.Queries;
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

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var users = await _mediator.Send(new ListAllUsersQuery(), ct);
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email.ToString(),
            Name = u.Name,
            Roles = u.Roles.ToString(),
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        });
        return Ok(userDtos);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto request, CancellationToken ct)
    {
        var command = new Application.Users.Commands.UpdateUser.UpdateUserCommand(id, request.Name, request.IsActive);
        var result = await _mediator.Send(command, ct);
        if (!result) return NotFound();
        var updatedUser = await _mediator.Send(new Application.Users.Queries.GetUserById.GetUserByIdQuery(id), ct);
        if (updatedUser is null) return NotFound();
        var userDto = new UserDto
        {
            Id = updatedUser.Id,
            Email = updatedUser.Email,
            Name = updatedUser.Name,
            Roles = string.Join(",", updatedUser.Roles.Select(r => r.ToString())),
            IsActive = updatedUser.IsActive,
            CreatedAt = updatedUser.CreatedAt,
            UpdatedAt = updatedUser.UpdatedAt
        };
        return Ok(userDto);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var command = new Application.Users.Commands.DeleteUser.DeleteUserCommand(id);
        var result = await _mediator.Send(command, ct);
        if (!result) return NotFound();
        return NoContent();
    }
}
