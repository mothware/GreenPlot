using GreenPlot.Domain.Enums;

namespace GreenPlot.Application.DTOs;

public record PlantingSummaryDto(
    Guid Id,
    Guid VarietyId,
    string VarietyName,
    string PlantCommonName,
    string PlantFamily,
    PlantingState State,
    PlantingMethod Method,
    DateOnly StartDate
);

public record PlantingDetailDto(
    Guid Id,
    Guid OwnerId,
    Guid BedId,
    Guid VarietyId,
    string VarietyName,
    string PlantCommonName,
    string PlantFamily,
    Guid? SeedLotId,
    Guid SeasonId,
    string SeasonName,
    DateOnly StartDate,
    PlantingMethod Method,
    PlantingState State,
    string? Notes,
    bool CompanionWarningOverridden,
    string? CompanionOverrideReason,
    List<ActivityDto> Activities,
    List<CompanionWarningDto> CompanionWarnings,
    SeedStartDatesDto? SeedStartDates
);

public record SeedStartDatesDto(
    DateOnly? IndoorStartMin,
    DateOnly? IndoorStartMax,
    DateOnly? OutdoorTransplantMin,
    DateOnly? OutdoorTransplantMax,
    DateOnly? DirectSowMin,
    DateOnly? DirectSowMax,
    DateOnly? EstimatedFirstHarvest
);

public record CompanionWarningDto(
    Guid AdjacentPlantingId,
    string AdjacentVarietyName,
    string AdjacentPlantName,
    string Effect,
    string? Reasoning
);
