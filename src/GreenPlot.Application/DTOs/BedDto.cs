using GreenPlot.Domain.Enums;

namespace GreenPlot.Application.DTOs;

public record BedSummaryDto(
    Guid Id,
    string Name,
    BedType Type,
    BedShape Shape,
    decimal WidthFeet,
    decimal LengthFeet,
    decimal GridCellSizeFeet,
    SunRequirement? SunExposure,
    bool IsArchived,
    int PlantingCount
);

public record BedDetailDto(
    Guid Id,
    Guid GardenId,
    string Name,
    BedType Type,
    BedShape Shape,
    decimal WidthFeet,
    decimal LengthFeet,
    decimal GridCellSizeFeet,
    SunRequirement? SunExposure,
    string? SoilNotes,
    bool IsArchived,
    List<BedCellDto> Cells,
    List<PlantingSummaryDto> Plantings
);

public record BedCellDto(
    Guid Id,
    int X,
    int Y,
    PlantingSummaryDto? Planting
);
