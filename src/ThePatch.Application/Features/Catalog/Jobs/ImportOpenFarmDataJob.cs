using ThePatch.Application.Common.Interfaces;
using ThePatch.Domain.Entities;
using ThePatch.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ThePatch.Application.Features.Catalog.Jobs;

public class ImportOpenFarmDataJob
{
    private readonly IApplicationDbContext _db;
    private readonly IOpenFarmService _openFarm;
    private readonly ILogger<ImportOpenFarmDataJob> _logger;

    // Comprehensive crop list — full common vegetable/herb/flower/fruit catalog
    private static readonly string[] CommonCrops =
    [
        // Solanaceae
        "tomato", "cherry-tomato", "pepper", "bell-pepper", "hot-pepper",
        "eggplant", "tomatillo", "potato",
        // Cucurbitaceae
        "cucumber", "zucchini", "summer-squash", "winter-squash", "pumpkin",
        "butternut-squash", "acorn-squash", "watermelon", "cantaloupe",
        "honeydew", "bitter-melon", "luffa",
        // Brassicaceae
        "broccoli", "cauliflower", "cabbage", "kale", "collards",
        "brussels-sprouts", "kohlrabi", "bok-choy", "arugula", "radish",
        "turnip", "rutabaga", "daikon", "mustard-greens", "watercress",
        // Fabaceae
        "green-bean", "pole-bean", "bush-bean", "pea", "snow-pea",
        "snap-pea", "edamame", "lima-bean", "black-eyed-pea", "fava-bean",
        // Apiaceae
        "carrot", "parsnip", "celery", "celeriac", "fennel",
        "dill", "cilantro", "parsley", "chervil", "anise",
        // Alliaceae
        "onion", "garlic", "leek", "chive", "shallot",
        "scallion", "elephant-garlic",
        // Asteraceae
        "lettuce", "romaine", "butterhead-lettuce", "oakleaf-lettuce",
        "endive", "radicchio", "artichoke", "sunflower", "chamomile",
        "calendula", "marigold", "echinacea",
        // Amaranthaceae
        "spinach", "swiss-chard", "beet", "amaranth", "quinoa",
        // Lamiaceae (herbs)
        "basil", "mint", "peppermint", "spearmint", "oregano",
        "thyme", "rosemary", "sage", "lavender", "lemon-balm",
        "marjoram", "catnip", "hyssop",
        // Poaceae
        "corn", "sweet-corn", "popcorn",
        // Other vegetables
        "sweet-potato", "rhubarb", "sorrel", "asparagus", "okra",
        // Fruits
        "strawberry", "raspberry", "blackberry", "blueberry",
        // Flowers / companion plants
        "nasturtium",
        // Cover crops
        "clover", "buckwheat", "winter-rye", "vetch",
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
        _logger.LogInformation("Starting OpenFarm import for {Count} crop slugs", CommonCrops.Length);
        int imported = 0, skipped = 0, failed = 0;

        foreach (var slug in CommonCrops)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var crop = await _openFarm.GetCropAsync(slug, ct)
                    ?? (await _openFarm.SearchCropsAsync(slug.Replace('-', ' '), ct)).FirstOrDefault();

                if (crop == null) { skipped++; continue; }

                var exists = await _db.Plants.AnyAsync(
                    p => p.ExternalId == crop.Slug && p.ExternalSource == "OpenFarm", ct);

                if (exists) { skipped++; continue; }

                _db.Plants.Add(new Plant
                {
                    CommonName = crop.Name,
                    ScientificName = string.Empty,
                    Family = string.Empty,
                    Category = InferCategory(slug),
                    Lifecycle = PlantLifecycle.Annual,
                    SunRequirement = ParseSunReq(crop.SunRequirements),
                    WaterNeeds = "moderate",
                    SpacingInches = crop.SpreadDiameter.HasValue ? (decimal)crop.SpreadDiameter : null,
                    IsGlobal = true,
                    ExternalId = crop.Slug,
                    ExternalSource = "OpenFarm",
                    Notes = crop.Description
                });

                imported++;
                if (imported % 20 == 0)
                    await _db.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to import slug: {Slug}", slug);
                failed++;
            }

            await Task.Delay(300, ct);
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation(
            "OpenFarm import complete — imported: {I}, skipped: {S}, failed: {F}",
            imported, skipped, failed);
    }

    private static SunRequirement ParseSunReq(string? raw) =>
        raw?.ToLower() switch
        {
            "full sun" => SunRequirement.FullSun,
            "partial sun" or "partial shade" => SunRequirement.PartialSun,
            "full shade" => SunRequirement.Shade,
            _ => SunRequirement.FullSun
        };

    private static PlantCategory InferCategory(string slug) => slug switch
    {
        var s when s.Contains("marigold") || s.Contains("sunflower") || s.Contains("lavender")
                || s.Contains("nasturtium") || s.Contains("chamomile") || s.Contains("calendula")
                || s.Contains("echinacea") => PlantCategory.Flower,
        var s when s.Contains("clover") || s.Contains("buckwheat") || s.Contains("rye")
                || s.Contains("vetch") => PlantCategory.CoverCrop,
        var s when s.Contains("basil") || s.Contains("mint") || s.Contains("oregano")
                || s.Contains("thyme") || s.Contains("rosemary") || s.Contains("sage")
                || s.Contains("dill") || s.Contains("cilantro") || s.Contains("parsley")
                || s.Contains("chive") || s.Contains("fennel") || s.Contains("anise")
                || s.Contains("lemon-balm") || s.Contains("marjoram") || s.Contains("catnip")
                || s.Contains("hyssop") || s.Contains("chervil") => PlantCategory.Herb,
        var s when s.Contains("strawberry") || s.Contains("raspberry") || s.Contains("blackberry")
                || s.Contains("blueberry") || s.Contains("watermelon") || s.Contains("cantaloupe")
                || s.Contains("honeydew") => PlantCategory.Fruit,
        _ => PlantCategory.Vegetable
    };
}
