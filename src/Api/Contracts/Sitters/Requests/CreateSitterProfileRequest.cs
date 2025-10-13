namespace Api.Contracts.Sitters.Requests;

public record CreateSitterProfileRequest(
    Guid UserId,
    string? Bio = null,
    decimal? BaseRateAmount = null,
    string? BaseRateCurrency = "USD",
    int ServicesOffered = 1 // Default to basic pet sitting
);
