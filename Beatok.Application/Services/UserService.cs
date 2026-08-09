using AutoMapper;
using Beatok.Application.DTOs.User;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class UserService(IApplicationDbContext context, IMapper mapper, 
    IStorage storage): IUserService
{
    public async Task EnsureExistsAsync(string userId, string name, bool isAnonymous)
    {
        var user = await context.Users.FindAsync(userId);

        if (user == null)
        {
            await context.EnsureUserExistsAsync(userId, name, isAnonymous);
            return;
        }

        if (user.IsAnonymous && !isAnonymous)
        {
            user.IsAnonymous = false;
            user.Name = name;

            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsAdminAsync(string userId)
    {
        return await context.Users
            .AnyAsync(u => u.Id == userId && u.Role == UserRole.Administrator);
    }
    
    public PictureUploadDto GenerateUploadUrl(string fileExtension, string contentType)
    {
        if (!fileExtension.StartsWith('.'))
        {
            fileExtension = $".{fileExtension}";
        }

        var fileKey = $"{Guid.NewGuid()}{fileExtension}";
        var uploadUrl = storage.GeneratePresignedUploadUrl($"pictures/{fileKey}", TimeSpan.FromMinutes(15), contentType);

        return new PictureUploadDto
        {
            UploadUrl = uploadUrl,
            FileKey = fileKey
        };
    }
    
    public async Task UpdateLastActiveAtAsync(string userId)
    {
        await context.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(s => 
                s.SetProperty(u => u.LastActiveAt, DateTime.UtcNow));
    }

    public async Task<ProfileDto> GetByIdAsync(string userId, int? year = null)
    {
        var user = await context.Users
                       .Include(u => u.Participations)
                       .ThenInclude(p => p.Submissions)
                       .ThenInclude(s => s.Lobby)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new UserNotFoundException();

        var availableYears = await context.Lobbies
            .AsNoTracking()
            .Where(l => l.Participants.Any(p => p.UserId == userId))
            .Where(l => l.EndedAt > DateTime.MinValue) // ignores active lobbies
            .Select(l => l.EndedAt.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();

        DateTime startDate;
        int dayCount;

        if (year.HasValue)
        {
            startDate = new DateTime(year.Value, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            dayCount = DateTime.IsLeapYear(year.Value) ? 366 : 365;
        }
        else
        {
            dayCount = 365;
            var todayUtc = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
            startDate = todayUtc.AddDays(-(dayCount - 1));
        }

        var nextDay = startDate.AddDays(dayCount);

        var lobbyCounts = await context.Lobbies
            .AsNoTracking()
            .Where(l => l.Participants.Any(p => p.UserId == userId))
            .Where(l => l.EndedAt >= startDate && l.EndedAt < nextDay)
            .GroupBy(l => l.EndedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Date, x => x.Count);

        var activity = new List<ActivityDayDto>(capacity: dayCount);

        for (int i = 0; i < dayCount; i++)
        {
            var currentDate = startDate.AddDays(i);
            activity.Add(new ActivityDayDto
            {
                Date = currentDate.ToString("yyyy-MM-dd"),
                Count = lobbyCounts.GetValueOrDefault(currentDate, 0)
            });
        }
        
        int totalGames = user.Participations
            .Count(p => !p.IsKicked);
        int totalWins = user.Participations.Count(p =>
            !p.IsKicked &&
            p.Submissions.Any(s =>
                s.Lobby != null &&
                s.Lobby.WinningSubmissionId == s.Id));

        var profile = mapper.Map<ProfileDto>(user);
        profile.Activity = activity;
        profile.AvailableYears = availableYears;
        profile.Wins = totalWins;
        profile.WinRate = totalGames == 0
            ? 0
            : (double)totalWins / totalGames * 100;

        return profile;
    }

    public async Task<MeDto> GetMeAsync(string userId)
    {
        var user = await context.Users.FindAsync(userId)
            ?? throw new UserNotFoundException();
        return mapper.Map<MeDto>(user);
    }

    public async Task UpdateAsync(string userId, UserUpdateDto dto)
    {
        var user = await context.Users.FindAsync(userId)
            ?? throw new UserNotFoundException();
        if (dto.Name is not null)
        {
            user.Name = dto.Name;
        }
        if (dto.Picture is not null)
        {
            await storage.DeleteFileAsync($"pictures/{user.Picture}");
            user.Picture = dto.Picture;
        }
        await context.SaveChangesAsync();
    }
}