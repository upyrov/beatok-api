using Beatok.Application.DTOs.Score;
using FluentValidation;

namespace Beatok.Application.Validators.Score;

public class UpdateScoreDtoValidator : AbstractValidator<UpdateScoreDto>
{
    public UpdateScoreDtoValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Value is required");
    }
}