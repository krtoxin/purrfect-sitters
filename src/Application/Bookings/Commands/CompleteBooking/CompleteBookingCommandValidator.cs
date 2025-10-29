using FluentValidation;

namespace Application.Bookings.Commands.CompleteBooking;

public class CompleteBookingCommandValidator : AbstractValidator<CompleteBookingCommand>
{
    public CompleteBookingCommandValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
    }
}