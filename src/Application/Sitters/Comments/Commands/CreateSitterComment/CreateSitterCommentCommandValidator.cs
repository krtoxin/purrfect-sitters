using FluentValidation;

namespace Application.Sitters.Comments.Commands.CreateSitterComment;

public class CreateSitterCommentCommandValidator : AbstractValidator<CreateSitterCommentCommand>
{
    public CreateSitterCommentCommandValidator()
    {
        RuleFor(x => x.SitterProfileId).NotEmpty();
        RuleFor(x => x.Content)
            .NotEmpty()
            .Must(s => !string.IsNullOrWhiteSpace(s))
            .WithMessage("Content cannot be empty.")
            .MaximumLength(1000);
    }
}