using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatok.Infrastructure.Persistence.Configurations;

public class ParticipationEntityTypeConfiguration : IEntityTypeConfiguration<Participation>
{
    public void Configure(EntityTypeBuilder<Participation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsConnected)
            .IsRequired();

        builder.Property(x => x.JoinedAt)
           .IsRequired();

        // Ensure a user can only be in a specific lobby once
        builder.HasIndex(x => new { x.UserId, x.LobbyId })
            .IsUnique();

        builder.HasIndex(x => x.ConnectionId);
    }
}