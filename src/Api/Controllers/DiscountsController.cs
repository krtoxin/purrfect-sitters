using Api.DTOs;
using Application.Common.Interfaces;
using Domain.Sitters;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/discounts")]
public class DiscountsController : ControllerBase
{
    private readonly IServiceDiscountRepository _repo;
    private readonly IUnitOfWork _uow;

    public DiscountsController(IServiceDiscountRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    [HttpGet]
    public async Task<ActionResult<List<DiscountDto>>> List([FromQuery] string? category = null)
    {
        if (!string.IsNullOrWhiteSpace(category) && Enum.TryParse<SitterServiceType>(category, true, out var cat))
        {
            var active = await _repo.ListActiveByCategoryAsync(cat, DateTime.UtcNow);
            return Ok(active.Select(Map));
        }
        var all = await _repo.ListAllAsync();
        return Ok(all.Select(Map));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DiscountDto>> Get(Guid id)
    {
        var d = await _repo.GetByIdAsync(id);
        if (d == null) return NotFound();
        return Ok(Map(d));
    }

    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateDiscountDto dto)
    {
        if (!Enum.TryParse<SitterServiceType>(dto.Category, true, out var cat))
            return Problem("Invalid category", statusCode: 422);
        var result = ServiceDiscount.Create(cat, dto.Percentage, dto.ExpiresAt);
        if (!result.IsSuccess)
            return Problem(result.Error ?? "Validation failed", statusCode: 422);
        var entity = result.Value!;
        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, new { id = entity.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateDiscountDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();
        var update = entity.Update(dto.Percentage, dto.ExpiresAt);
        if (!update.IsSuccess)
            return Problem(update.Error ?? "Validation failed", statusCode: 422);
        await _repo.UpdateAsync(entity);
        await _uow.SaveChangesAsync();
        return Ok(Map(entity));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return NotFound();
        await _repo.DeleteAsync(id);
        await _uow.SaveChangesAsync();
        return NoContent();
    }

    private static DiscountDto Map(ServiceDiscount d) => new()
    {
        Id = d.Id,
        Category = d.Category.ToString(),
        Percentage = d.Percentage,
        ExpiresAt = d.ExpiresAt,
        CreatedAt = d.CreatedAt
    };
}
