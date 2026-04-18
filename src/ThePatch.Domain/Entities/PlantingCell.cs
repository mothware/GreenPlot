using ThePatch.Domain.Common;

namespace ThePatch.Domain.Entities;

/// <summary>
/// Junction table linking a Planting to the specific BedCells it occupies.
/// </summary>
public class PlantingCell : BaseEntity
{
    public Guid PlantingId { get; set; }
    public Guid BedCellId { get; set; }
    public int PlantCount { get; set; } = 1;

    public Planting Planting { get; set; } = null!;
    public BedCell BedCell { get; set; } = null!;
}
