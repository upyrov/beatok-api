namespace Beatok.Application.DTOs.User;

public record ProfileDto : UserDto
{
    public List<ActivityDayDto> Activity { get; set; } = [];
    public List<int> AvailableYears { get; set; } = [];
}