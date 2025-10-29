using FluentValidation;

namespace Application.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.PetId).NotEmpty();
        RuleFor(x => x.SitterProfileId).NotEmpty();
        RuleFor(x => x.StartUtc)
            .LessThan(x => x.EndUtc)
            .WithMessage("StartUtc must be earlier than EndUtc.");
        RuleFor(x => x.BaseAmount).GreaterThan(0);

        RuleForEach(x => x.CareInstructionTexts)
            .MaximumLength(400)
            .When(x => x.CareInstructionTexts is not null);

        RuleFor(x => x.ServiceFeePercent)
            .GreaterThanOrEqualTo(0).When(x => x.ServiceFeePercent.HasValue)
            .LessThanOrEqualTo(100).When(x => x.ServiceFeePercent.HasValue);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3) // ISO 4217 style
            .Matches("^[A-Za-z]{3}$")
            .WithMessage("Currency must be a 3-letter code like USD or EUR.");
    }
}