using Beatok.Application.DTOs.Sound;
using FluentValidation;

namespace Beatok.Application.Validators.Sound;

public class CreateSoundDtoValidator : AbstractValidator<CreateSoundDto>
{
    public CreateSoundDtoValidator()
    {
        RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required");
    }
}