using Beatok.Application.DTOs.Genre;
using FluentValidation;

namespace Beatok.Application.Validators;

public class CreateGenreDtoValidator: AbstractValidator<CreateGenreDto>
{
    public CreateGenreDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must be less than 100 characters")
            .Matches(@"^[a-zA-Z0-9\s]+$").WithMessage("Name must contain only letters, numbers, and spaces");
    }
}