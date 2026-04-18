using ThePatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ThePatch.Infrastructure.Data.Configurations;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.WeightGrams).HasPrecision(10, 2);
        builder.Property(a => a.PhotoUrls)
            .HasColumnType("jsonb");

        builder.HasIndex(a => a.PlantingId);
        builder.HasIndex(a => a.OccurredAt);

        builder.HasOne(a => a.Planting)
            .WithMany(p => p.Activities)
            .HasForeignKey(a => a.PlantingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.CorrectsActivity)
            .WithMany()
            .HasForeignKey(a => a.CorrectsActivityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
