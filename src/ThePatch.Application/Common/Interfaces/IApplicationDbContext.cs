using ThePatch.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ThePatch.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Plant> Plants { get; }
    DbSet<Variety> Varieties { get; }
    DbSet<Garden> Gardens { get; }
    DbSet<Bed> Beds { get; }
    DbSet<BedCell> BedCells { get; }
    DbSet<SeedLot> SeedLots { get; }
    DbSet<Season> Seasons { get; }
    DbSet<Planting> Plantings { get; }
    DbSet<PlantingCell> PlantingCells { get; }
    DbSet<Activity> Activities { get; }
    DbSet<CompanionRule> CompanionRules { get; }
    DbSet<Reminder> Reminders { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
