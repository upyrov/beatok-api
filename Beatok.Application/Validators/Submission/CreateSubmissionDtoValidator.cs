using Beatok.Application.DTOs.Submission;
using FluentValidation;

namespace Beatok.Application.Validators.Submission;

public class CreateSubmissionDtoValidator : AbstractValidator<CreateSubmissionDto>
{
    public CreateSubmissionDtoValidator()
    {
        RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required");
        
        RuleFor(x => x.DurationSeconds)
            .NotEmpty().WithMessage("DurationSeconds is required");
    }
}