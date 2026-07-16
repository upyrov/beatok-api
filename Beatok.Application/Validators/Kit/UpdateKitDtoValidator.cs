using Beatok.Application.DTOs.Kit;
using FluentValidation;

namespace Beatok.Application.Validators.Kit;

public class UpdateKitDtoValidator : AbstractValidator<UpdateKitDto>
{
    public UpdateKitDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must be less than 100 characters");
    }
}