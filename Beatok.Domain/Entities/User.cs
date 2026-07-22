namespace Beatok.Domain.Entities;

public enum UserRole
{
    Administrator,
    Player
}

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public bool IsAnonymous { get; set; } 
    public UserRole Role { get; set; }
    public DateTime? LastActiveAt { get; set; }
    public ICollection<Lobby> OwnedLobbies { get; set; } = [];
    public ICollection<Participation> Participations { get; set; } = [];
    public ICollection<Comment> CommentsAuthored { get; set; } = [];
    public ICollection<Comment> ProfileComments { get; set; } = [];
}