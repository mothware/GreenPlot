using GreenPlot.Domain.Common;
using GreenPlot.Domain.Enums;

namespace GreenPlot.Domain.Entities;

public class Planting : BaseEntity
{
    public Guid OwnerId { get; set; }
    public Guid BedId { get; set; }
    public Guid VarietyId { get; set; }
    public Guid? SeedLotId { get; set; }
    public Guid SeasonId { get; set; }
    public DateOnly StartDate { get; set; }
    public PlantingMethod Method { get; set; }
    public PlantingState State { get; set; } = PlantingState.Planned;
    public string? Notes { get; set; }
    public bool CompanionWarningOverridden { get; set; } = false;
    public string? CompanionOverrideReason { get; set; }

    public Bed Bed { get; set; } = null!;
    public Variety Variety { get; set; } = null!;
    public SeedLot? SeedLot { get; set; }
    public Season Season { get; set; } = null!;
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public ICollection<PlantingCell> PlantingCells { get; set; } = new List<PlantingCell>();
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
}
