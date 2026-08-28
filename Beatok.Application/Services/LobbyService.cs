using AutoMapper;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.Genre;
using Beatok.Application.DTOs.Lobby;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class LobbyService(IApplicationDbContext context,
    IValidator<CreateLobbyDto> validator, 
    IBackgroundJobClient backgroundJobClient,
    ILobbyNotifier lobbyNotifier, 
    IMapper mapper,
    ILobbyLifecycleService lobbyLifecycleService
    ) : ILobbyService
{
    public async Task<Guid> CreateAsync(CreateLobbyDto dto, string ownerId)
    {
        var fluentValidation = await validator.ValidateAsync(dto);
        if (!fluentValidation.IsValid)
        {
            throw new ValidationException(fluentValidation.Errors);
        }

        if (!await context.Users.AnyAsync(u => u.Id == ownerId))
            throw new NotFoundException("User not found");
        if (!await context.Genres.AnyAsync(g => g.Id == dto.GenreId))
            throw new NotFoundException("Genre not found");
        var activeLobbyCount = await context.Participation
            .Where(p => p.UserId == ownerId && p.Lobby!.State != LobbyState.Ended && !p.IsKicked)
            .CountAsync();
        if (activeLobbyCount >= 2)
            throw new BadRequestException("User cannot join more than 2 active lobbies");

        var lobby = new Lobby
        {
            Name = dto.Name,
            OwnerId = ownerId,
            GenreId = dto.GenreId,
            ParticipantLimit = dto.ParticipantLimit,
            SubmissionTime = dto.SubmissionTime
        };

        await context.Lobbies.AddAsync(lobby);
        await context.SaveChangesAsync();
        return lobby.Id;
    }

    public async Task<DetailedLobbyDto> JoinAsync(Guid lobbyId, string userId, string connectionId)
    {
        var lobby = await context.Lobbies
                        .Include(l => l.Participants)
                        .FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException("User not found");

        var participant = lobby.Participants.FirstOrDefault(p =>
            p.UserId == userId && p.LobbyId == lobbyId);

        if (participant is null)
        {
            await HandleNewParticipantAsync(lobby, user, connectionId);
        }
        else
        {
            await HandleExistingParticipantAsync(participant, connectionId);
        }

        if (lobby.Participants.Count(p => !p.IsKicked) >= lobby.ParticipantLimit)
        {
            await lobbyLifecycleService.StartAsync(lobbyId, lobby.OwnerId);
        }

        var lobbyWithParticipants = await context.Lobbies
            .AsNoTracking() // prevents presigned URLs from saving to DB
            .AsSplitQuery() // prevents Cartesian Explosion
            .Include(l => l.Genre)
            .Include(l => l.Owner)
            .Include(l => l.Participants)
                .ThenInclude(p => p.User)
            .Include(l => l.Participants)
                .ThenInclude(p => p.Scores)
            .Include(l => l.Submissions)
                .ThenInclude(s => s.Participant)
                    .ThenInclude(p => p!.User)
            .Include(l => l.Sounds)
                .ThenInclude(s => s.Category)
            .FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");

        var currentItem = lobbyWithParticipants.State == LobbyState.Voting
            ? await context.LobbyPlaybackItems
                .Where(x => x.LobbyId == lobbyId && x.StartedAt != null)
                .OrderByDescending(x => x.StartedAt)
                .FirstOrDefaultAsync()
            : null;
        
        var lobbyDto = mapper.Map<DetailedLobbyDto>(lobbyWithParticipants);
        lobbyDto.CurrentPlaybackItem =
            mapper.Map<LobbyPlaybackItemDto?>(currentItem);
        return lobbyDto;       
    }

    private async Task HandleNewParticipantAsync(Lobby lobby, User user, string connectionId)
    {
        var activeLobbyCount = await context.Participation
            .CountAsync(p => p.UserId == user.Id && p.Lobby!.State != LobbyState.Ended && !p.IsKicked);

        if (activeLobbyCount >= 2)
            throw new BadRequestException("User cannot join more than 2 active lobbies");
        if (lobby.State != LobbyState.Waiting)
            throw new BadRequestException("Lobby is already started");
        if (lobby.Participants.Count >= lobby.ParticipantLimit)
            throw new BadRequestException("Lobby is full");

        var newParticipant = new Participation
        {
            LobbyId = lobby.Id,
            UserId = user.Id,
            User = user,
            ConnectionId = connectionId
        };

        lobby.Participants.Add(newParticipant);
        await context.Participation.AddAsync(newParticipant);
        await context.SaveChangesAsync();

        await lobbyNotifier.ParticipantJoinedAsync(lobby.Id, mapper.Map<ParticipationDto>(newParticipant));   
    }

    private async Task HandleExistingParticipantAsync(Participation participant, string connectionId)
    {
        if (participant.IsKicked)
        {
            throw new BadRequestException("You have been kicked from the lobby");
        }
            
        participant.ConnectionId = connectionId;
        await RejoinAsync(participant);
    }

    private async Task RejoinAsync(Participation participant)
    {
        if (!string.IsNullOrEmpty(participant.DisconnectJobId))
        {
            backgroundJobClient.Delete(participant.DisconnectJobId);
            participant.DisconnectJobId = null;
        }
        participant.IsConnected = true;
        await context.SaveChangesAsync();
        await lobbyNotifier.ParticipantConnectedAsync(participant.LobbyId, participant.UserId);
    }

    public async Task LeaveAsync(Guid lobbyId, string userId)
    {
        var lobby = await context.Lobbies
                        .Include(l => l.Participants)
                        .FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        var user = await context.Users.FindAsync(userId)
            ?? throw new NotFoundException("User not found");
        var participant = lobby.Participants
            .FirstOrDefault(p => p.UserId == user.Id) ??
                          throw new NotFoundException("User not found in lobby");

        if (lobby.State == LobbyState.Waiting)
        {
            await HandleNotStartedLeaveAsync(lobby, participant);
            await lobbyNotifier.ParticipantLeftAsync(lobby.Id, userId);
        }
        else
        {
            await lobbyNotifier.ParticipantDisconnectedAsync(lobby.Id, userId);
        }
    }

    private async Task HandleNotStartedLeaveAsync(Lobby lobby, Participation participant)
    {
        var wasOwner = lobby.OwnerId == participant.UserId;
        
        lobby.Participants.Remove(participant);
        
        if (lobby.Participants.Count(p => !p.IsKicked) == 0)
        {
            context.Lobbies.Remove(lobby);
            await context.SaveChangesAsync();
            return;
        }

        if (wasOwner)
        {
            var newOwner = lobby.Participants
                .OrderBy(p => p.JoinedAt)
                .First();
            lobby.OwnerId = newOwner.UserId;
            await context.SaveChangesAsync();
            await lobbyNotifier.OwnerChangedAsync(lobby.Id, newOwner.UserId);
            return;
        }
        await context.SaveChangesAsync();
    }

    public async Task DisconnectAsync(string connectionId)
    {
        var participant = await context.Participation
            .Include(p => p.Lobby)
            .ThenInclude(l => l!.Participants)
            .Where(p => p.ConnectionId == connectionId)
            .FirstOrDefaultAsync();

        if (participant != null)
        {
            participant.IsConnected = false;
            participant.ConnectionId = null;
            
            if (!participant.IsKicked)
            {
                if (participant.Lobby!.State == LobbyState.Waiting)
                {
                    var jobId = backgroundJobClient.Schedule<ILobbyService>(
                        s => s.HandleDisconnectTimeoutAsync(participant.LobbyId, participant.UserId),
                        TimeSpan.FromSeconds(3));
        
                    participant.DisconnectJobId = jobId;
                }
                else
                {
                    participant.DisconnectJobId = null;
                }
                await context.SaveChangesAsync();
                
                if (participant.Lobby!.State != LobbyState.Waiting)
                {
                    await lobbyNotifier.ParticipantDisconnectedAsync(participant.LobbyId, participant.UserId);
                }
            }
        }
    }
    
    public async Task HandleDisconnectTimeoutAsync(Guid lobbyId, string userId)
    {
        var lobby = await context.Lobbies
            .Include(l => l.Participants)
            .FirstOrDefaultAsync(l => l.Id == lobbyId);

        if (lobby == null || lobby.State != LobbyState.Waiting)
            return;

        var participant = lobby.Participants.FirstOrDefault(p => p.UserId == userId);
        if (participant != null && !participant.IsConnected)
        {
            await HandleNotStartedLeaveAsync(lobby, participant);
            await lobbyNotifier.ParticipantLeftAsync(lobby.Id, userId);
        }
    }

    public async Task<IEnumerable<LobbyDto>> GetAllAsync(LobbyFilterDto filter, string? userId)
    {
        var query = context.Lobbies
            .Include(l => l.Genre)
            .Include(l => l.Owner)
            .Include(l => l.Participants)
            .AsQueryable();

        query = query.Where(l =>
            (userId != null && l.State != LobbyState.Ended && l.Participants
                .Any(p => p.UserId == userId && !p.IsKicked))
            ||
            (l.State == LobbyState.Waiting && l.Participants.Count < l.ParticipantLimit &&
             (userId == null || l.Participants.All(p => p.UserId != userId || !p.IsKicked)))
        );


        var filteredQuery = ApplyFilter(query, filter);
        var lobbies = await filteredQuery.ToListAsync();

        var dtos = mapper.Map<List<LobbyDto>>(lobbies);

        if (userId != null)
        {
            for (int i = 0; i < lobbies.Count; i++)
            {
                dtos[i].IsJoined = lobbies[i].Participants
                    .Any(p => p.UserId == userId && !p.IsKicked);
            }
        }

        return dtos;
    }

    public async Task<List<ArchivedLobbyDto>> GetByUserIdAsync(string userId, DateTime date)
    {
        var userExists = await context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
            throw new UserNotFoundException();

        var utcDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
        var lobbies = await context.Lobbies
            .AsNoTracking()
            .Include(l => l.Genre)
            .Include(l => l.Participants)
                .ThenInclude(p => p.Submissions)
            .Where(l => l.Participants.Any(p => p.UserId == userId && !p.IsKicked))
            .Where(l => l.EndedAt.Date == utcDate)
            .ToListAsync();
        
        return lobbies.Select(l => new ArchivedLobbyDto
        {
            Id = l.Id,
            Name = l.Name,
            CreatedAt = l.CreatedAt,
            SubmissionStartedAt = l.SubmissionStartedAt,
            VotingStartedAt = l.VotingStartedAt,
            EndedAt = l.EndedAt,
            Genre = new GenreDto
            {
                Id = l.GenreId,
                Name = l.Genre!.Name
            },
            IsWinner = l.Participants.Any(p =>
                !p.IsKicked &&
                p.UserId == userId &&
                p.Submissions.Any(s => s.Id == l.WinningSubmissionId)),
            ParticipantCount = l.Participants.Count(p => !p.IsKicked),
            
        }).ToList();
    }

    private IQueryable<Lobby> ApplyFilter(IQueryable<Lobby> query, LobbyFilterDto filter)
    {
        if (!string.IsNullOrEmpty(filter.Name))
        {
            query = query.Where(l => l.Name.ToLower().Contains(filter.Name.ToLower().Trim()));
        }

        if (filter.GenreId.HasValue)
        {
            query = query.Where(l => l.GenreId == filter.GenreId);
        }

        return query;
    }

    public async Task KickAsync(Guid lobbyId, string userId, string targetUserId)
    {
        var lobby = await context.Lobbies
                .Include(l => l.Participants)
                .FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        if (lobby.OwnerId != userId)
            throw new BadRequestException("You are not the owner of this lobby");
        var participant = lobby.Participants
            .FirstOrDefault(p => p.UserId == targetUserId) ??
                throw new NotFoundException("User not found in lobby");

        participant.IsKicked = true;
        await context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(participant.ConnectionId))
        {
            await lobbyNotifier.KickedReceivedAsync(participant.ConnectionId);
        }
        await lobbyNotifier.ParticipantLeftAsync(lobby.Id, targetUserId);
    }

    public async Task SendMessageAsync(Guid lobbyId, string userId, string content)
    {
        var lobby = await context.Lobbies
                .Include(l => l.Participants)
                .FirstOrDefaultAsync(l => l.Id == lobbyId)
            ?? throw new NotFoundException("Lobby not found");
        var participant = lobby.Participants
            .FirstOrDefault(p => p.UserId == userId && !p.IsKicked) ??
                          throw new NotFoundException("User not found in lobby");
        
        if (string.IsNullOrWhiteSpace(content))
            throw new BadRequestException("Message cannot be empty");
        if (content.Length > 250)
            throw new BadRequestException("Message cannot be longer than 250 characters");

        await lobbyNotifier.MessageReceivedAsync(lobbyId, userId, content);
    }
}