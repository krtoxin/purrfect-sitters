namespace Api.Contracts.Bookings.Requests;

public sealed record CreateBookingRequest(
    Guid PetId,
    Guid SitterProfileId,
    DateTime StartUtc,
    DateTime EndUtc,
    decimal BaseAmount,
    decimal? ServiceFeePercent,
    string Currency,
    IEnumerable<string>? CareInstructionTexts);