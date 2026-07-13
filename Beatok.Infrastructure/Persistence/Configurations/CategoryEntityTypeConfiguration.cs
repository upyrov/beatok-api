using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatok.Infrastructure.Persistence.Configurations;

public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(x => x.Kit)
            .WithMany()
            .HasForeignKey(x => x.KitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}