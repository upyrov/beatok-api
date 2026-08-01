namespace Beatok.Domain.Entities;

public enum UserRole
{
    Administrator,
    Player
}

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public bool IsAnonymous { get; set; } 
    public string? Picture {  get; set; }
    public UserRole Role { get; set; }
    public DateTime? LastActiveAt { get; set; }
    public double Mu { get; set; } = 25.0;
    public double Sigma { get; set; } = 8.333;
    public int Rating { get; set; } = 0;
    public ICollection<Lobby> OwnedLobbies { get; set; } = [];
    public ICollection<Participation> Participations { get; set; } = [];
    public ICollection<Comment> CommentsAuthored { get; set; } = [];
    public ICollection<Comment> ProfileComments { get; set; } = [];
}