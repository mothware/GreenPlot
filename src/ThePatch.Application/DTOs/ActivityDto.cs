using ThePatch.Domain.Enums;

namespace ThePatch.Application.DTOs;

public record ActivityDto(
    Guid Id,
    Guid PlantingId,
    ActivityType Type,
    DateTime OccurredAt,
    int? Quantity,
    decimal? WeightGrams,
    string? Notes,
    List<string> PhotoUrls
);
