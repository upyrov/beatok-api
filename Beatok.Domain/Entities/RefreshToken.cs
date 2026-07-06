namespace Beatok.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TokenHash { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public DateTime Expires { get; set; }
}