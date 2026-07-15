using Beatok.Application.DTOs.Submission;
using FluentValidation;

namespace Beatok.Application.Validators.Submission;

public class UpdateSubmissionDtoValidator : AbstractValidator<UpdateSubmissionDto>
{
    public UpdateSubmissionDtoValidator()
    {
        RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required");
    }
}