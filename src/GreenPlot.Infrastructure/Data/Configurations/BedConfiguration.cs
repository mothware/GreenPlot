using GreenPlot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenPlot.Infrastructure.Data.Configurations;

public class BedConfiguration : IEntityTypeConfiguration<Bed>
{
    public void Configure(EntityTypeBuilder<Bed> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).IsRequired().HasMaxLength(200);
        builder.Property(b => b.WidthFeet).HasPrecision(8, 2);
        builder.Property(b => b.LengthFeet).HasPrecision(8, 2);
        builder.Property(b => b.GridCellSizeFeet).HasPrecision(5, 2);

        builder.HasIndex(b => b.GardenId);

        builder.HasMany(b => b.Cells)
            .WithOne(c => c.Bed)
            .HasForeignKey(c => c.BedId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.Plantings)
            .WithOne(p => p.Bed)
            .HasForeignKey(p => p.BedId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
