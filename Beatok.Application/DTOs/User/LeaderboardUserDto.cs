namespace Beatok.Application.DTOs.User;

public record LeaderboardUserDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public string? Picture { get; set; }
    public int Rating { get; set; }
    public int Wins { get; set; }
}