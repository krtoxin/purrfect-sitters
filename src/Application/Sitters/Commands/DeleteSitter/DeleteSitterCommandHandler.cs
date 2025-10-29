using Application.Common.Interfaces;
using MediatR;

namespace Application.Sitters.Commands.DeleteSitter;

public class DeleteSitterCommandHandler : IRequestHandler<DeleteSitterCommand, bool>
{
    private readonly ISitterRepository _sitters;
    private readonly IUnitOfWork _uow;
    public DeleteSitterCommandHandler(ISitterRepository sitters, IUnitOfWork uow)
    {
        _sitters = sitters;
        _uow = uow;
    }

    public async Task<bool> Handle(DeleteSitterCommand request, CancellationToken cancellationToken)
    {
        var sitter = await _sitters.GetByIdAsync(request.Id, cancellationToken);
        if (sitter is null) return false;
        await _sitters.DeleteAsync(sitter, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
