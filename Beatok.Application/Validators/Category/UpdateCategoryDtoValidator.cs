using Beatok.Application.DTOs.Category;
using FluentValidation;

namespace Beatok.Application.Validators.Category;

public class UpdateCategoryDtoValidator : AbstractValidator<CategoryUpdateDto>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
          .NotEmpty().WithMessage("Name is required")
          .MaximumLength(100).WithMessage("Name must be less than 100 characters");
        
        RuleFor(x => x.RandomSoundsCount)
            .GreaterThan(0).WithMessage("RandomSoundsCount must be greater than 0");
    }
}