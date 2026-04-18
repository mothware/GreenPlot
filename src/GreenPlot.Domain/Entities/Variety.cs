using GreenPlot.Domain.Common;
using GreenPlot.Domain.Enums;

namespace GreenPlot.Domain.Entities;

public class Variety : BaseEntity
{
    public Guid PlantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? DaysToMaturity { get; set; }
    public int? DaysToGerminate { get; set; }
    public decimal? SowingDepthInches { get; set; }
    public decimal? SpacingInches { get; set; }
    public SunRequirement? SunRequirement { get; set; }
    public string? DiseaseResistance { get; set; }
    public string? FlavorNotes { get; set; }
    public string? ColorNotes { get; set; }
    public string? Source { get; set; }
    public string? Notes { get; set; }
    public bool IsGlobal { get; set; } = true;
    public Guid? OwnerId { get; set; }

    public Plant Plant { get; set; } = null!;
    public ICollection<SeedLot> SeedLots { get; set; } = new List<SeedLot>();
    public ICollection<Planting> Plantings { get; set; } = new List<Planting>();
}
