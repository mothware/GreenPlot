using ThePatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ThePatch.Infrastructure.Data.Configurations;

public class GardenConfiguration : IEntityTypeConfiguration<Garden>
{
    public void Configure(EntityTypeBuilder<Garden> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Name).IsRequired().HasMaxLength(200);
        builder.Property(g => g.ZipCode).HasMaxLength(20);
        builder.Property(g => g.HardinessZone).HasMaxLength(10);

        builder.HasIndex(g => g.OwnerId);

        builder.HasOne(g => g.Owner)
            .WithMany(u => u.Gardens)
            .HasForeignKey(g => g.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(g => g.Beds)
            .WithOne(b => b.Garden)
            .HasForeignKey(b => b.GardenId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
