using FluentValidation;

namespace Application.Pets.Commands.CreatePet;

public class CreatePetCommandValidator : AbstractValidator<CreatePetCommand>
{
    public CreatePetCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("Owner ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Pet name is required and must not exceed 100 characters.");

        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(BeValidPetType)
            .WithMessage("Pet type must be a valid type (Dog, Cat, Bird, Fish, Other).");

        RuleFor(x => x.Breed)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Breed))
            .WithMessage("Breed must not exceed 100 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes must not exceed 500 characters.");
    }

    private static bool BeValidPetType(string type)
    {
        return Enum.TryParse<Domain.Pets.PetType>(type, ignoreCase: true, out _);
    }
}
