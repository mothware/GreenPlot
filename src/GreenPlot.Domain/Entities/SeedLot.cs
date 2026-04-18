using GreenPlot.Domain.Common;

namespace GreenPlot.Domain.Entities;

public class SeedLot : BaseEntity
{
    public Guid OwnerId { get; set; }
    public Guid VarietyId { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public string? Supplier { get; set; }
    public int? PacketSize { get; set; }
    public int? PacketSeedCount { get; set; }
    public decimal? ViabilityPercent { get; set; }
    public int? PacketYear { get; set; }
    public string? Notes { get; set; }
    public int? SeedsRemaining { get; set; }

    public Variety Variety { get; set; } = null!;
    public ICollection<Planting> Plantings { get; set; } = new List<Planting>();
}
