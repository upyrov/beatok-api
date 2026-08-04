using Beatok.Application.DTOs.Sound;
using FluentValidation;

namespace Beatok.Application.Validators.Sound;

public class UpdateSoundDtoValidator : AbstractValidator<SoundUpdateDto>
{
    public UpdateSoundDtoValidator()
    {
        RuleFor(x => x.Value).NotEmpty().WithMessage("Value is required");
    }
}