using Beatok.Application.DTOs.Lobby;
using FluentValidation;

namespace Beatok.Application.Validators;

public class CreateLobbyDtoValidator: AbstractValidator<CreateLobbyDto>
{
    public CreateLobbyDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be greater than 3 characters")
            .MaximumLength(100).WithMessage("Name must be less than 100 characters");

        RuleFor(x => x.ParticipantLimit)
            .InclusiveBetween((short)2, short.MaxValue)
            .WithMessage($"Participant limit must be between 2 and {short.MaxValue}");
        
        RuleFor(x => x.SubmissionTime)
            .NotEmpty().WithMessage("Submission time limit is required")
            .Must(t => t.TotalMinutes >= 3 && t.TotalMinutes <= 30)
            .WithMessage("Submission time limit must be between 3 and 30 minutes");
    }
}