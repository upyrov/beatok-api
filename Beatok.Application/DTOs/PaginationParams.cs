namespace Beatok.Application.DTOs;

public record PaginationParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}