using Api.Contracts.Bookings.Requests;
using Api.Contracts.Bookings.Responses;
using Api.Contracts.Common;
using Api.DTOs;
using Api.Mappings;
using Application.Bookings.Commands.AcceptBooking;
using Application.Bookings.Commands.CancelByOwner;
using Application.Bookings.Commands.CancelBySitter;
using Application.Bookings.Commands.CompleteBooking;
using Application.Bookings.Commands.CreateBooking;
using Application.Bookings.Models;
using Application.Bookings.Queries.GetBookingById;
using Application.Bookings.Queries.ListBookingsForOwner;
using Application.Bookings.Queries;
using Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator) => _mediator = mediator;


    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookingDto request, CancellationToken ct)
    {
        var result = await _mediator.Send(new Application.Bookings.Commands.UpdateBooking.UpdateBookingCommand(
            id,
            DateTime.UtcNow, 
            DateTime.UtcNow.AddDays(1),
            0, 
            0,
            "USD",
            request.CareInstructions.Split(',', StringSplitOptions.RemoveEmptyEntries)), ct);
        if (!result)
            return NotFound();
        return Ok();
    }


    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new Application.Bookings.Commands.DeleteBooking.DeleteBookingCommand(id), ct);
        if (!result)
            return NotFound();
        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateBookingCommand(
            request.PetId,
            request.SitterProfileId,
            request.StartUtc,
            request.EndUtc,
            request.BaseAmount,
            request.ServiceFeePercent,
            request.Currency,
            request.CareInstructionTexts), ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var model = await _mediator.Send(new GetBookingByIdQuery(id), ct);
        if (model is null) return NotFound();
        return Ok(model.ToResponse());
    }

    [HttpGet("owner/{ownerId:guid}")]
    [ProducesResponseType(typeof(PagedResponse<BookingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListForOwner(Guid ownerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new ListBookingsForOwnerQuery(ownerId, page, pageSize), ct);
        var response = new PagedResponse<BookingResponse>(
            result.Items.Select(m => m.ToResponse()),
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);
        return Ok(response);
    }

    [HttpPost("{id:guid}/accept")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Accept(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new AcceptBookingCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new CompleteBookingCommand(id), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/cancel/owner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelByOwner(Guid id, [FromBody] CancelBookingRequest body, CancellationToken ct)
    {
        await _mediator.Send(new CancelBookingByOwnerCommand(id, body.Reason), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/cancel/sitter")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CancelBySitter(Guid id, [FromBody] CancelBookingRequest body, CancellationToken ct)
    {
        await _mediator.Send(new CancelBookingBySitterCommand(id, body.Reason), ct);
        return NoContent();
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var bookings = await _mediator.Send(new ListAllBookingsQuery(), ct);
        var bookingDtos = bookings.Select(b => new BookingDto
        {
            Id = b.Id,
            PetId = b.PetId,
            OwnerId = b.OwnerId,
            SitterId = b.SitterProfileId,
            StartDate = b.StartUtc,
            EndDate = b.EndUtc,
            Status = b.Status.ToString(),
            TotalAmount = b.Price.TotalAmount,
            ServiceFee = b.Price.ServiceFeeAmount,
            Currency = b.Price.Currency,
            ServiceType = b.ServiceType.ToString(),
            CreatedAt = b.CreatedAt
        });
        return Ok(bookingDtos);
    }
}