using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatok.Infrastructure.Persistence.Configurations
{
    public class SoundEntityTypeConfiguration : IEntityTypeConfiguration<Sound>
    {
        public void Configure(EntityTypeBuilder<Sound> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value).IsRequired();

            builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}