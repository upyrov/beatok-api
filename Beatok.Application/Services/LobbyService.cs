using Beatok.Application.DTOs.Lobby;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;

namespace Beatok.Application.Services;

public class LobbyService(IUnitOfWork unitOfWork,
    IValidator<CreateLobbyDto> validator): ILobbyService
{
    public async Task CreateAsync(CreateLobbyDto dto, Guid ownerId)
    {
        var fluentValidation = await validator.ValidateAsync(dto);
        if (!fluentValidation.IsValid)
        {
            throw new ValidationException(fluentValidation.Errors);
        }

        var owner = await unitOfWork.Users.GetByIdAsync(ownerId);

        if (owner == null)
        {
            throw new NotFoundException("User not found");
        }

        await unitOfWork.Lobbies.AddAsync(new Lobby
        {
            Name = dto.Name,
            OwnerId = owner.Id,
            ParticipantLimit = dto.ParticipantLimit,
            SubmissionTimeLimit = dto.SubmissionTimeLimit,
            VotingTimeLimit = dto.VotingTimeLimit
        });
        await unitOfWork.SaveChangesAsync();
    }
}