using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatok.Infrastructure.Persistence.Configurations;

public class RefreshTokenEntityTypeConfiguration: IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(t => t.TokenHash)
            .HasMaxLength(255);

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();
        
        builder.HasIndex(x => x.Expires);
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
    }
}