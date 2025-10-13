using FluentValidation;

namespace Application.Sitters.Commands.CreateSitterProfile;

public class CreateSitterProfileCommandValidator : AbstractValidator<CreateSitterProfileCommand>
{
    public CreateSitterProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.Bio)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Bio is required and must not exceed 1000 characters.");

        RuleFor(x => x.BaseRateAmount)
            .GreaterThan(0)
            .WithMessage("Base rate amount must be greater than 0.");

        RuleFor(x => x.BaseRateCurrency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Base rate currency must be a valid 3-letter currency code.");

        RuleFor(x => x.ServicesOffered)
            .NotEmpty()
            .WithMessage("At least one service must be offered.");

        RuleForEach(x => x.ServicesOffered)
            .Must(BeValidServiceType)
            .WithMessage("Each service type must be valid (PetSitting, DogWalking, PetGrooming, PetTraining, PetBoarding).");
    }

    private static bool BeValidServiceType(string serviceType)
    {
        return Enum.TryParse<Domain.Sitters.SitterServiceType>(serviceType, ignoreCase: true, out _);
    }
}
