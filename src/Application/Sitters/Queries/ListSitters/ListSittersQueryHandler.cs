using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Sitters.Models;
using MediatR;

namespace Application.Sitters.Queries.ListSitters;

public class ListSittersQueryHandler : IRequestHandler<ListSittersQuery, PagedResult<SitterProfileReadModel>>
{
    private readonly ISitterProfileRepository _sitterProfileRepository;

    public ListSittersQueryHandler(ISitterProfileRepository sitterProfileRepository)
    {
        _sitterProfileRepository = sitterProfileRepository;
    }

    public async Task<PagedResult<SitterProfileReadModel>> Handle(ListSittersQuery request, CancellationToken cancellationToken)
    {
        var sitterProfiles = await _sitterProfileRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        
        var readModels = sitterProfiles.Select(sitterProfile => new SitterProfileReadModel(
            sitterProfile.Id,
            sitterProfile.UserId,
            sitterProfile.Bio,
            sitterProfile.BaseRate?.Amount ?? 0,
            sitterProfile.BaseRate?.Currency ?? "USD",
            new[] { sitterProfile.ServicesOffered },
            sitterProfile.CreatedAt,
            sitterProfile.CreatedAt));

        return PagedResult<SitterProfileReadModel>.Create(
            readModels,
            request.Page,
            request.PageSize,
            sitterProfiles.Count);
    }
}
