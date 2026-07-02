namespace Beatok.Application.DTOs.Error;

public class ErrorDto
{
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
}