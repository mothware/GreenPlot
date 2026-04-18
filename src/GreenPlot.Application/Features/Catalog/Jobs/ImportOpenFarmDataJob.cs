using GreenPlot.Application.Common.Interfaces;
using GreenPlot.Domain.Entities;
using GreenPlot.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GreenPlot.Application.Features.Catalog.Jobs;

public class ImportOpenFarmDataJob
{
    private readonly IApplicationDbContext _db;
    private readonly IOpenFarmService _openFarm;
    private readonly ILogger<ImportOpenFarmDataJob> _logger;

    private static readonly string[] CommonCrops = [
        "tomato", "pepper", "cucumber", "squash", "zucchini", "basil",
        "lettuce", "spinach", "kale", "broccoli", "cauliflower", "carrot",
        "radish", "beet", "onion", "garlic", "pea", "bean", "corn",
        "pumpkin", "watermelon", "cantaloupe", "eggplant", "okra",
        "Swiss chard", "arugula", "cilantro", "parsley", "dill", "oregano",
        "thyme", "rosemary", "mint", "sage", "lavender", "sunflower",
        "marigold", "nasturtium", "pansies", "sweet potato", "potato",
        "asparagus", "rhubarb", "strawberry", "raspberry", "blueberry",
        "fennel", "leek", "celery"
    ];

    public ImportOpenFarmDataJob(
        IApplicationDbContext db,
        IOpenFarmService openFarm,
        ILogger<ImportOpenFarmDataJob> logger)
    {
        _db = db;
        _openFarm = openFarm;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting OpenFarm data import for {Count} crops", CommonCrops.Length);

        foreach (var cropName in CommonCrops)
        {
            try
            {
                var results = await _openFarm.SearchCropsAsync(cropName, ct);
                var crop = results.FirstOrDefault();
                if (crop == null) continue;

                var existing = await _db.Plants
                    .FirstOrDefaultAsync(p => p.ExternalId == crop.Slug && p.ExternalSource == "OpenFarm", ct);

                if (existing != null) continue;

                var plant = new Plant
                {
                    CommonName = crop.Name,
                    ScientificName = string.Empty,
                    Family = string.Empty,
                    Category = PlantCategory.Vegetable,
                    Lifecycle = PlantLifecycle.Annual,
                    SunRequirement = ParseSunReq(crop.SunRequirements),
                    WaterNeeds = "moderate",
                    SpacingInches = crop.SpreadDiameter.HasValue ? (decimal?)crop.SpreadDiameter : null,
                    IsGlobal = true,
                    ExternalId = crop.Slug,
                    ExternalSource = "OpenFarm",
                    Notes = crop.Description
                };

                _db.Plants.Add(plant);
                _logger.LogInformation("Imported plant: {Name}", crop.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to import crop: {Name}", cropName);
            }
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("OpenFarm import complete");
    }

    private static SunRequirement ParseSunReq(string? raw) =>
        raw?.ToLower() switch
        {
            "full sun" => SunRequirement.FullSun,
            "partial sun" or "partial shade" => SunRequirement.PartialSun,
            "full shade" => SunRequirement.Shade,
            _ => SunRequirement.FullSun
        };
}
