using ThePatch.Domain.Enums;

namespace ThePatch.Application.DTOs;

public record PlantDto(
    Guid Id,
    string CommonName,
    string ScientificName,
    string Family,
    PlantCategory Category,
    PlantLifecycle Lifecycle,
    SunRequirement SunRequirement,
    string WaterNeeds,
    int? DaysToGerminateMin,
    int? DaysToGerminateMax,
    int? DaysToMaturityMin,
    int? DaysToMaturityMax,
    decimal? SowingDepthInches,
    decimal? SpacingInches,
    string? HardinessZoneMin,
    string? HardinessZoneMax,
    string? Notes,
    bool IsGlobal,
    int VarietyCount
);

public record VarietyDto(
    Guid Id,
    Guid PlantId,
    string PlantCommonName,
    string Name,
    int? DaysToMaturity,
    int? DaysToGerminate,
    decimal? SowingDepthInches,
    decimal? SpacingInches,
    SunRequirement? SunRequirement,
    string? DiseaseResistance,
    string? FlavorNotes,
    string? ColorNotes,
    string? Source,
    string? Notes,
    bool IsGlobal
);
