namespace Beatok.Application.DTOs;

public record LeaderboardQuery
{
    public string SortBy { get; set; } = "rating";
}