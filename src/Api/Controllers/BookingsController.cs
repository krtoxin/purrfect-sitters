using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
using Application.Bookings.Commands.UpdateBooking;
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
        // If a status change requested, use dedicated commands (no row-version passed from controller).
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (string.Equals(request.Status, "Accepted", StringComparison.OrdinalIgnoreCase))
            {
                await _mediator.Send(new AcceptBookingCommand(id), ct);
            }
            else if (string.Equals(request.Status, "Completed", StringComparison.OrdinalIgnoreCase))
            {
                await _mediator.Send(new CompleteBookingCommand(id), ct);
            }

            // Re-fetch after status change to ensure latest state
            var afterStatus = await _mediator.Send(new GetBookingByIdQuery(id), ct);
            if (afterStatus is null) return NotFound();
            return Ok(afterStatus.ToResponse());
        }

        // No status change — partial update: preserve existing times/price and update care instructions
        var current = await _mediator.Send(new GetBookingByIdQuery(id), ct);
        if (current is null) return NotFound();

        var startUtc = current.StartUtc;
        var endUtc = current.EndUtc;
        var baseAmount = current.Price.BaseAmount;
        var serviceFeePercent = current.Price.ServiceFeePercent.GetValueOrDefault();
        var currency = current.Price.Currency;

        var careInstructions = (request.CareInstructions ?? string.Empty)
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .ToArray();

        var result = await _mediator.Send(new UpdateBookingCommand(
            id,
            startUtc,
            endUtc,
            baseAmount,
            serviceFeePercent,
            currency,
            careInstructions), ct);

        if (!result) return NotFound();

        var updated = await _mediator.Send(new GetBookingByIdQuery(id), ct);
        if (updated is null) return NotFound();

        return Ok(updated.ToResponse());
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
        var model = await _mediator.Send(new GetBookingByIdQuery(id), ct);
        if (model is null) return NotFound();

        // Don't pass row-version bytes from controller; handler handles concurrency.
        await _mediator.Send(new AcceptBookingCommand(id), ct);
        // Re-fetch after status change to ensure latest state (for persistence, not for response)
        var afterStatus = await _mediator.Send(new GetBookingByIdQuery(id), ct);
        if (afterStatus is null) return NotFound();
        return NoContent();
    }

    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        var model = await _mediator.Send(new GetBookingByIdQuery(id), ct);
        if (model is null) return NotFound();

        await _mediator.Send(new CompleteBookingCommand(id), ct);
        // Re-fetch after status change to ensure latest state (for persistence, not for response)
        var afterStatus = await _mediator.Send(new GetBookingByIdQuery(id), ct);
        if (afterStatus is null) return NotFound();
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
    [ProducesResponseType(typeof(IEnumerable<BookingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var bookings = await _mediator.Send(new ListAllBookingsQuery(), ct);
        var responses = bookings
            .Select(b => b.ToReadModel())
            .Select(rm => rm.ToResponse());
        return Ok(responses);
    }
}