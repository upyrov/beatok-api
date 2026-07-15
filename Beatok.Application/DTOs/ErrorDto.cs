namespace Beatok.Application.DTOs;

public record ErrorDto
{
    public required string Message { get; set; }
    public int StatusCode { get; set; }
}