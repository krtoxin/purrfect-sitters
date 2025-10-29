using MediatR;

namespace Application.Sitters.Commands.UpdateSitter;

public sealed record UpdateSitterCommand(
    Guid Id,
    string Bio,
    decimal BaseRateAmount,
    string BaseRateCurrency,
    string[] ServicesOffered
) : IRequest<bool>;
