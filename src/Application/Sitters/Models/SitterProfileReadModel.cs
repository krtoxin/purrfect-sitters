using Domain.Sitters;

namespace Application.Sitters.Models;

public record SitterProfileReadModel(
    Guid Id,
    Guid UserId,
    string Bio,
    decimal BaseRateAmount,
    string BaseRateCurrency,
    IEnumerable<SitterServiceType> ServicesOffered,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public bool IsActive { get; init; } = true;
    public decimal AverageRating { get; init; } = 0m;
    public int CompletedBookings { get; init; } = 0;
    public int ServicesOfferedInt => (int)ServicesOffered.FirstOrDefault();
}
