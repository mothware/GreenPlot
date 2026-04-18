using ThePatch.Domain.Enums;

namespace ThePatch.Application.DTOs;

public record CalendarEventDto(
    Guid Id,
    string Title,
    DateTime StartDate,
    DateTime? EndDate,
    string EventType,
    Guid? PlantingId,
    Guid? GardenId,
    Guid? BedId,
    string? VarietyName,
    string? PlantName,
    string? GardenName,
    string? BedName
);

public record ReminderDto(
    Guid Id,
    string Type,
    DateTime ScheduledFor,
    ReminderStatus Status,
    ReminderDeliveryChannel DeliveryChannel,
    string? Message,
    Guid? PlantingId
);
