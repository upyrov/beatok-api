namespace Beatok.Application.DTOs.User;

public record ProfileDto : UserDto
{
    public List<ActivityDayDto> Activity { get; set; } = [];
    public List<int> AvailableYears { get; set; } = [];
    public int Wins { get; set; }
    public double WinRate { get; set; }
}