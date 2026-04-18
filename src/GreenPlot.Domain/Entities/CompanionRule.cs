using GreenPlot.Domain.Common;
using GreenPlot.Domain.Enums;

namespace GreenPlot.Domain.Entities;

public class CompanionRule : BaseEntity
{
    public Guid PlantAId { get; set; }
    public Guid PlantBId { get; set; }
    public CompanionEffect Effect { get; set; }
    public int Strength { get; set; } = 1;
    public string? Reasoning { get; set; }
    public CompanionSourceType SourceType { get; set; }

    public Plant PlantA { get; set; } = null!;
    public Plant PlantB { get; set; } = null!;
}
