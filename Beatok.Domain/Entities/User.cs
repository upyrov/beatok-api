namespace Beatok.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public bool IsAnonymous { get; set; } = false;
    public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
}