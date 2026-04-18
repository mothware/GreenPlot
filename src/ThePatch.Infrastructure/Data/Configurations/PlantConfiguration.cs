using ThePatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ThePatch.Infrastructure.Data.Configurations;

public class PlantConfiguration : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.CommonName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.ScientificName).HasMaxLength(200);
        builder.Property(p => p.Family).HasMaxLength(100);
        builder.Property(p => p.WaterNeeds).HasMaxLength(50);
        builder.Property(p => p.ExternalId).HasMaxLength(200);
        builder.Property(p => p.ExternalSource).HasMaxLength(50);
        builder.Property(p => p.SowingDepthInches).HasPrecision(5, 2);
        builder.Property(p => p.SpacingInches).HasPrecision(5, 2);

        builder.HasIndex(p => new { p.CommonName, p.IsGlobal });
        builder.HasIndex(p => p.ExternalId);

        builder.HasMany(p => p.Varieties)
            .WithOne(v => v.Plant)
            .HasForeignKey(v => v.PlantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.CompanionRulesAsPlantA)
            .WithOne(r => r.PlantA)
            .HasForeignKey(r => r.PlantAId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.CompanionRulesAsPlantB)
            .WithOne(r => r.PlantB)
            .HasForeignKey(r => r.PlantBId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
