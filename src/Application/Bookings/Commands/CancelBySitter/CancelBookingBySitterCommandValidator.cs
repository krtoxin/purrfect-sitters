using Domain.Bookings;
using FluentValidation;

namespace Application.Bookings.Commands.CancelBySitter;

public class CancelBookingBySitterCommandValidator : AbstractValidator<CancelBookingBySitterCommand>
{
    public CancelBookingBySitterCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x.Reason).IsInEnum();
    }
}