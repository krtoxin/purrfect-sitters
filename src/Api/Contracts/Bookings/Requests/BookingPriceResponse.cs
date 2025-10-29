namespace Api.Contracts.Bookings.Responses;

public sealed record BookingPriceResponse(
    decimal BaseAmount,
    decimal? ServiceFeePercent,
    decimal ServiceFeeAmount,
    decimal TotalAmount,
    string Currency);