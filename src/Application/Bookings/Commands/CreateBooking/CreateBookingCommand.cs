using MediatR;

namespace Application.Bookings.Commands.CreateBooking;

public sealed record CreateBookingCommand(
    Guid PetId,
    Guid SitterProfileId,
    DateTime StartUtc,
    DateTime EndUtc,
    decimal BaseAmount,
    decimal? ServiceFeePercent,
    string Currency,
    IEnumerable<string>? CareInstructionTexts
) : IRequest<Guid>;