using MediatR;

namespace Application.Bookings.Commands.UpdateBooking;

public sealed record UpdateBookingCommand(
    Guid Id,
    DateTime StartUtc,
    DateTime EndUtc,
    decimal BaseAmount,
    decimal ServiceFeePercent,
    string Currency,
    string[] CareInstructionTexts,
    byte[] RowVersion
) : IRequest<bool>;
