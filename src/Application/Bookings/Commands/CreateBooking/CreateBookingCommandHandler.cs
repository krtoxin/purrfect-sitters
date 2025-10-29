using Application.Common.Interfaces;
using Domain.Bookings;
using MediatR;

namespace Application.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
{
    private readonly IBookingRepository _bookings;
    private readonly IPetRepository _pets;
    private readonly ISitterProfileRepository _sitters;
    private readonly IUnitOfWork _uow;

    public CreateBookingCommandHandler(
        IBookingRepository bookings,
        IPetRepository pets,
        ISitterProfileRepository sitters,
        IUnitOfWork uow)
    {
        _bookings = bookings;
        _pets = pets;
        _sitters = sitters;
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken ct)
    {
        var pet = await _pets.GetByIdAsync(request.PetId, ct)
            ?? throw new InvalidOperationException("Pet not found.");

        var sitter = await _sitters.GetByIdAsync(request.SitterProfileId, ct)
            ?? throw new InvalidOperationException("Sitter profile not found.");

        var serviceFeePercent = request.ServiceFeePercent ?? 0m;

        var price = BookingPrice.Create(
            request.BaseAmount,
            serviceFeePercent,
            request.Currency);

        var booking = Booking.Create(
            Guid.NewGuid(),
            pet.Id,
            pet.OwnerId,
            sitter.Id,
            request.StartUtc,
            request.EndUtc,
            price,
            sitter.ServicesOffered,
            request.CareInstructionTexts ?? Array.Empty<string>());

        await _bookings.AddAsync(booking, ct);
        await _uow.SaveChangesAsync(ct);
        return booking.Id;
    }
}