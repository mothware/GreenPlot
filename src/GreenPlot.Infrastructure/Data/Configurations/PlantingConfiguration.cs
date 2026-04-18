using GreenPlot.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenPlot.Infrastructure.Data.Configurations;

public class PlantingConfiguration : IEntityTypeConfiguration<Planting>
{
    public void Configure(EntityTypeBuilder<Planting> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.OwnerId);
        builder.HasIndex(p => p.BedId);
        builder.HasIndex(p => p.SeasonId);

        builder.HasOne(p => p.Variety)
            .WithMany(v => v.Plantings)
            .HasForeignKey(p => p.VarietyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.SeedLot)
            .WithMany(s => s.Plantings)
            .HasForeignKey(p => p.SeedLotId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Season)
            .WithMany(s => s.Plantings)
            .HasForeignKey(p => p.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
