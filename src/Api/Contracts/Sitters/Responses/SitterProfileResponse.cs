namespace Api.Contracts.Sitters.Responses;

public record SitterProfileResponse(
    Guid Id,
    Guid UserId,
    string? Bio,
    bool IsActive,
    decimal AverageRating,
    decimal? BaseRateAmount,
    string? BaseRateCurrency,
    int ServicesOffered,
    int CompletedBookings,
    DateTime CreatedAt
);
