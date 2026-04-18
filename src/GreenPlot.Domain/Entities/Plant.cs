using GreenPlot.Domain.Common;
using GreenPlot.Domain.Enums;

namespace GreenPlot.Domain.Entities;

public class Plant : BaseEntity
{
    public string CommonName { get; set; } = string.Empty;
    public string ScientificName { get; set; } = string.Empty;
    public string Family { get; set; } = string.Empty;
    public PlantCategory Category { get; set; }
    public PlantLifecycle Lifecycle { get; set; }
    public SunRequirement SunRequirement { get; set; }
    public string WaterNeeds { get; set; } = string.Empty;
    public int? DaysToGerminateMin { get; set; }
    public int? DaysToGerminateMax { get; set; }
    public int? DaysToMaturityMin { get; set; }
    public int? DaysToMaturityMax { get; set; }
    public decimal? SowingDepthInches { get; set; }
    public decimal? SpacingInches { get; set; }
    public string? HardinessZoneMin { get; set; }
    public string? HardinessZoneMax { get; set; }
    public string? Notes { get; set; }
    public string? ExternalId { get; set; }
    public string? ExternalSource { get; set; }
    public bool IsGlobal { get; set; } = true;
    public Guid? OwnerId { get; set; }

    public ICollection<Variety> Varieties { get; set; } = new List<Variety>();
    public ICollection<CompanionRule> CompanionRulesAsPlantA { get; set; } = new List<CompanionRule>();
    public ICollection<CompanionRule> CompanionRulesAsPlantB { get; set; } = new List<CompanionRule>();
}
