using FluentValidation;

namespace Application.Bookings.Commands.AcceptBooking;

public class AcceptBookingCommandValidator : AbstractValidator<AcceptBookingCommand>
{
    public AcceptBookingCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
    }
}