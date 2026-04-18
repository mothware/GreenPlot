using ThePatch.Domain.Common;
using ThePatch.Domain.Enums;

namespace ThePatch.Domain.Entities;

public class Activity : BaseEntity
{
    public Guid PlantingId { get; set; }
    public ActivityType Type { get; set; }
    public DateTime OccurredAt { get; set; }
    public int? Quantity { get; set; }
    public decimal? WeightGrams { get; set; }
    public string? Notes { get; set; }
    public List<string> PhotoUrls { get; set; } = new();
    public Guid? CorrectsActivityId { get; set; }

    public Planting Planting { get; set; } = null!;
    public Activity? CorrectsActivity { get; set; }
}
