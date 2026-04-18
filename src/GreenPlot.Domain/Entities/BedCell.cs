using GreenPlot.Domain.Common;

namespace GreenPlot.Domain.Entities;

public class BedCell : BaseEntity
{
    public Guid BedId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public Bed Bed { get; set; } = null!;
    public ICollection<PlantingCell> PlantingCells { get; set; } = new List<PlantingCell>();
}
