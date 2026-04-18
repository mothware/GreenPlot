namespace GreenPlot.Application.DTOs;

public record GardenDto(
    Guid Id,
    string Name,
    string? ZipCode,
    string? HardinessZone,
    DateOnly? LastFrostDate,
    DateOnly? FirstFrostDate,
    string? Notes,
    bool IsArchived,
    int BedCount,
    DateTime CreatedAt
);

public record GardenDetailDto(
    Guid Id,
    string Name,
    string? ZipCode,
    string? HardinessZone,
    DateOnly? LastFrostDate,
    DateOnly? FirstFrostDate,
    string? Notes,
    bool IsArchived,
    List<BedSummaryDto> Beds,
    DateTime CreatedAt
);
