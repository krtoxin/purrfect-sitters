
namespace Api.Contracts.Bookings.Responses
{
    public sealed record BookingCareInstructionResponse(Guid Id, string Text, DateTime CreatedAt);
}