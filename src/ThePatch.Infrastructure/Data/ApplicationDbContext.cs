using ThePatch.Application.Common.Interfaces;
using ThePatch.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ThePatch.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<Variety> Varieties => Set<Variety>();
    public DbSet<Garden> Gardens => Set<Garden>();
    public DbSet<Bed> Beds => Set<Bed>();
    public DbSet<BedCell> BedCells => Set<BedCell>();
    public DbSet<SeedLot> SeedLots => Set<SeedLot>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<Planting> Plantings => Set<Planting>();
    public DbSet<PlantingCell> PlantingCells => Set<PlantingCell>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<CompanionRule> CompanionRules => Set<CompanionRule>();
    public DbSet<Reminder> Reminders => Set<Reminder>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Common.BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
