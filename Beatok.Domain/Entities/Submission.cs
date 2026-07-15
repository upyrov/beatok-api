namespace Beatok.Domain.Entities;

public class Submission
{
    public Guid Id = Guid.NewGuid();
    public required string Value { get; set; }
    public Guid ParticipantId { get; set; } 
    public Participation? Participant { get; set; }
}