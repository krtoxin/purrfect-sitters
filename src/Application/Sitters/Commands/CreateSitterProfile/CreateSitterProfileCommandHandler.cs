using Application.Common.Interfaces;
using Domain.Sitters;
using MediatR;

namespace Application.Sitters.Commands.CreateSitterProfile;

public class CreateSitterProfileCommandHandler : IRequestHandler<CreateSitterProfileCommand, Guid>
{
    private readonly ISitterProfileRepository _sitterProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSitterProfileCommandHandler(ISitterProfileRepository sitterProfileRepository, IUnitOfWork unitOfWork)
    {
        _sitterProfileRepository = sitterProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateSitterProfileCommand request, CancellationToken cancellationToken)
    {
        var serviceTypes = request.ServicesOffered
            .Select(service => Enum.Parse<SitterServiceType>(service, ignoreCase: true))
            .Aggregate(SitterServiceType.None, (acc, service) => acc | service);

        var sitterProfile = SitterProfile.Create(
            Guid.NewGuid(),
            request.UserId,
            request.Bio,
            new Domain.ValueObjects.Money(request.BaseRateAmount, request.BaseRateCurrency),
            serviceTypes);

        await _sitterProfileRepository.AddAsync(sitterProfile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return sitterProfile.Id;
    }
}
