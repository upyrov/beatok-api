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
        
        RuleFor(x => x.SubmissionTimeLimit)
            .NotEmpty().WithMessage("Submission time limit is required")
            .Must(t => t.TotalSeconds >= 10 && t.TotalMinutes <= 30)
            .WithMessage("Submission time limit must be between 10 and 20 minutes");
        
        RuleFor(x => x.VotingTimeLimit)
            .NotEmpty().WithMessage("Voting time limit is required")
            .Must(t => t.TotalSeconds >= 10 && t.TotalMinutes <= 20)
            .WithMessage("Voting time limit must be between 10 and 20 minutes");
    }
}