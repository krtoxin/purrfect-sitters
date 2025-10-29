using Api.Contracts.Sitters.Responses;
using Application.Sitters.Models;

namespace Api.Mappings;

public static class SitterApiMappings
{
    public static SitterProfileResponse ToResponse(this SitterProfileReadModel model)
        => new(model.Id, model.UserId, model.Bio, model.IsActive, model.AverageRating, 
               model.BaseRateAmount, model.BaseRateCurrency, model.ServicesOfferedInt, 
               model.CompletedBookings, model.CreatedAt);
}
