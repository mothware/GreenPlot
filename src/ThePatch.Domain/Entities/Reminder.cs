using ThePatch.Domain.Common;
using ThePatch.Domain.Enums;

namespace ThePatch.Domain.Entities;

public class Reminder : BaseEntity
{
    public Guid OwnerId { get; set; }
    public Guid? PlantingId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime ScheduledFor { get; set; }
    public ReminderStatus Status { get; set; } = ReminderStatus.Pending;
    public ReminderDeliveryChannel DeliveryChannel { get; set; }
    public string? Message { get; set; }
    public DateTime? SnoozedUntil { get; set; }

    public Planting? Planting { get; set; }
}
