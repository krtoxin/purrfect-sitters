using MediatR;

namespace Application.Sitters.Commands.DeleteSitter;

public sealed record DeleteSitterCommand(
    Guid Id
) : IRequest<bool>;
