namespace Beatok.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? TokenHash { get; set; }
    public string userId { get; set; }
    public User? User { get; set; }
    public DateTime Expires { get; set; }
}