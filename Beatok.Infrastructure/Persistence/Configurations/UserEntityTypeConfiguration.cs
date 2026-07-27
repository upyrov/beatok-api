using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatok.Infrastructure.Persistence.Configurations;

public class UserEntityTypeConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(x => x.Email)
            .IsRequired(false)
            .HasMaxLength(255);
        
        builder.Property(x => x.PasswordHash)
            .IsRequired(false);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.IsAnonymous)
            .IsRequired();

        builder.HasMany(x => x.OwnedLobbies)
            .WithOne(x => x.Owner)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Participations)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.CommentsAuthored)
            .WithOne(x => x.Author)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.ProfileComments)
           .WithOne(x => x.TargetUser)
           .HasForeignKey(x => x.TargetUserId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}