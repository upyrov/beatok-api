using Beatok.Application.DTOs.Score;
using FluentValidation;

namespace Beatok.Application.Validators.Score;

public class CreateScoreDtoValidator: AbstractValidator<CreateScoreDto>
{
    public CreateScoreDtoValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required");
        
        RuleFor(x => x.SubmissionId)
            .NotEmpty().WithMessage("SubmissionId is required");
    }
}