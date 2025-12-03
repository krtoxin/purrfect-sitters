using FluentValidation;

namespace Application.Sitters.Comments.Commands.UpdateSitterComment;

public class UpdateSitterCommentCommandValidator : AbstractValidator<UpdateSitterCommentCommand>
{
    public UpdateSitterCommentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Content)
            .NotEmpty()
            .Must(s => !string.IsNullOrWhiteSpace(s))
            .WithMessage("Content cannot be empty.")
            .MaximumLength(1000);
    }
}