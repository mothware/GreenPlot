using ThePatch.Domain.Common;
using ThePatch.Domain.Enums;

namespace ThePatch.Domain.Entities;

public class Bed : BaseEntity
{
    public Guid GardenId { get; set; }
    public string Name { get; set; } = string.Empty;
    public BedType Type { get; set; }
    public BedShape Shape { get; set; }
    public decimal WidthFeet { get; set; }
    public decimal LengthFeet { get; set; }
    public decimal GridCellSizeFeet { get; set; } = 1m;
    public SunRequirement? SunExposure { get; set; }
    public string? SoilNotes { get; set; }
    public bool IsArchived { get; set; } = false;
    public string? ShapePolygonJson { get; set; }

    public Garden Garden { get; set; } = null!;
    public ICollection<BedCell> Cells { get; set; } = new List<BedCell>();
    public ICollection<Planting> Plantings { get; set; } = new List<Planting>();
}
