using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatok.Infrastructure.Persistence.Configurations;

public class SubmissionEntityTypeConfiguration: IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.Participant)
            .WithMany(p => p.Submissions)
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.ParticipantId);
    }
}