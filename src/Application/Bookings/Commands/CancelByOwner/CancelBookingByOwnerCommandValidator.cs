using Domain.Bookings;
using FluentValidation;
using MediatR;

namespace Application.Bookings.Commands.CancelByOwner;

public class CancelBookingByOwnerCommandValidator : AbstractValidator<CancelBookingByOwnerCommand>
{
    public CancelBookingByOwnerCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x.Reason).IsInEnum();
    }
}