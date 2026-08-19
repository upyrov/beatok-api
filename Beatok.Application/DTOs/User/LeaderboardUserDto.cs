namespace Beatok.Application.DTOs.User;

public record LeaderboardUserDto : UserDto
{
    public int Wins { get; set; }
}