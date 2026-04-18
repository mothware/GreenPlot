using GreenPlot.Domain.Common;

namespace GreenPlot.Domain.Entities;

public class Season : BaseEntity
{
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public ICollection<Planting> Plantings { get; set; } = new List<Planting>();
}
