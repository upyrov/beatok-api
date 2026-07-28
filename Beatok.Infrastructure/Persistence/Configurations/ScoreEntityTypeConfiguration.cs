using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatok.Infrastructure.Persistence.Configurations;

public class ScoreEntityTypeConfiguration: IEntityTypeConfiguration<Score>
{
    public void Configure(EntityTypeBuilder<Score> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Participant)
            .WithMany(p => p.Scores)
            .HasForeignKey(x => x.ParticipationId);
        
        builder.HasOne(x => x.Lobby)
            .WithMany()
            .HasForeignKey(x => x.LobbyId);
        
        builder.HasOne(x => x.Submission)
            .WithMany(s => s.Scores)
            .HasForeignKey(x => x.SubmissionId);

        builder.HasIndex(x => new { x.SubmissionId, x.ParticipationId })
            .IsUnique();
    }
}