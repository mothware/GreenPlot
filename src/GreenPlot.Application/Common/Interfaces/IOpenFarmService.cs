namespace GreenPlot.Application.Common.Interfaces;

public interface IOpenFarmService
{
    Task<IReadOnlyList<OpenFarmCropDto>> SearchCropsAsync(string query, CancellationToken ct = default);
    Task<OpenFarmCropDto?> GetCropAsync(string slug, CancellationToken ct = default);
}

public record OpenFarmCropDto(
    string Slug,
    string Name,
    string? Description,
    string? SunRequirements,
    string? SowingMethod,
    int? RowSpacing,
    int? SpreadDiameter,
    int? HeightMaximum,
    List<string> Companions,
    List<string> Incompatibilities
);
