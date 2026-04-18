using GreenPlot.Domain.Common;

namespace GreenPlot.Domain.Entities;

public class Garden : BaseEntity
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
    public string? HardinessZone { get; set; }
    public DateOnly? LastFrostDate { get; set; }
    public DateOnly? FirstFrostDate { get; set; }
    public string? Notes { get; set; }
    public bool IsArchived { get; set; } = false;

    public ApplicationUser Owner { get; set; } = null!;
    public ICollection<Bed> Beds { get; set; } = new List<Bed>();
}
