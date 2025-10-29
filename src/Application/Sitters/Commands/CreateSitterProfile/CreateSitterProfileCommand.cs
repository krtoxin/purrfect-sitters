using MediatR;

namespace Application.Sitters.Commands.CreateSitterProfile;

public record CreateSitterProfileCommand(
    Guid UserId,
    string Bio,
    decimal BaseRateAmount,
    string BaseRateCurrency,
    IEnumerable<string> ServicesOffered) : IRequest<Guid>;
