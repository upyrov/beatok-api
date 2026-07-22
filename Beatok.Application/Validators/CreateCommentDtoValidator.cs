using Beatok.Application.DTOs.Comment;
using FluentValidation;

namespace Beatok.Application.Validators;

public class CreateCommentDtoValidator: AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(300).WithMessage("Content must be less than 300 characters");
    }
}