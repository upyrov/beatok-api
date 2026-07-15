using Beatok.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatok.Infrastructure.Persistence.Configurations
{
    public class KitEntityTypeConfiguration : IEntityTypeConfiguration<Kit>
    {
        public void Configure(EntityTypeBuilder<Kit> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(x => x.Genres)
                .WithMany(x => x.Kits)
                .UsingEntity<GenreKit>(
                    j => j
                        .HasOne(gk => gk.Genre)
                        .WithMany()
                        .HasForeignKey(gk => gk.GenreId),
                    j => j
                        .HasOne(gk => gk.Kit)
                        .WithMany()
                        .HasForeignKey(gk => gk.KitId),
                    j => j.HasIndex(t => new { t.KitId, t.GenreId }).IsUnique()
                );

            builder.HasMany(x => x.Categories)
                .WithOne(x => x.Kit)
                .HasForeignKey(x => x.KitId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}