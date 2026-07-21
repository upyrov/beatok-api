using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatok.Infrastructure.Persistence.Configurations;

public class CommentEntityTypeConfiguration: IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasOne(c => c.Author)
            .WithMany(u => u.CommentsAuthored)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.TargetUser)
            .WithMany(u => u.ProfileComments)
            .HasForeignKey(c => c.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.TargetUserId);
    }
}