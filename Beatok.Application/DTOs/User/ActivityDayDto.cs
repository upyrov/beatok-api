namespace Beatok.Application.DTOs.User;

public record ActivityDayDto
{
    public required string Date { get; set; }
    public int Count { get; set; }
}