using Application.Common.Interfaces;
using Application.Sitters.Models;
using MediatR;

namespace Application.Sitters.Queries.GetSitterById;

public class GetSitterByIdQueryHandler : IRequestHandler<GetSitterByIdQuery, SitterProfileReadModel?>
{
    private readonly ISitterProfileRepository _sitterProfileRepository;

    public GetSitterByIdQueryHandler(ISitterProfileRepository sitterProfileRepository)
    {
        _sitterProfileRepository = sitterProfileRepository;
    }

    public async Task<SitterProfileReadModel?> Handle(GetSitterByIdQuery request, CancellationToken cancellationToken)
    {
        var sitterProfile = await _sitterProfileRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (sitterProfile is null)
            return null;

        return new SitterProfileReadModel(
            sitterProfile.Id,
            sitterProfile.UserId,
            sitterProfile.Bio,
            sitterProfile.BaseRate?.Amount ?? 0,
            sitterProfile.BaseRate?.Currency ?? "USD",
            new[] { sitterProfile.ServicesOffered },
            sitterProfile.CreatedAt,
            sitterProfile.CreatedAt);
    }
}
