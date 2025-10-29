using Application.Common.Interfaces;
using MediatR;

namespace Application.Sitters.Commands.UpdateSitter;

public class UpdateSitterCommandHandler : IRequestHandler<UpdateSitterCommand, bool>
{
    private readonly ISitterProfileRepository _sitterProfiles;
    private readonly IUnitOfWork _uow;
    public UpdateSitterCommandHandler(ISitterProfileRepository sitterProfiles, IUnitOfWork uow)
    {
        _sitterProfiles = sitterProfiles;
        _uow = uow;
    }

    public async Task<bool> Handle(UpdateSitterCommand request, CancellationToken cancellationToken)
    {
    var profile = await _sitterProfiles.GetByIdAsync(request.Id, cancellationToken);
    if (profile is null) return false;
    profile.UpdateProfile(request.Bio, request.BaseRateAmount, request.BaseRateCurrency, request.ServicesOffered);
        await _uow.SaveChangesAsync(cancellationToken);
        return true;
    }
}
