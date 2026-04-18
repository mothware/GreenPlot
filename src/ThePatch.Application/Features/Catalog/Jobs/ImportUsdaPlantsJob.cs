using ThePatch.Application.Common.Interfaces;
using ThePatch.Domain.Entities;
using ThePatch.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ThePatch.Application.Features.Catalog.Jobs;

/// <summary>
/// Enriches existing catalog entries with USDA PLANTS taxonomy data (scientific name, family,
/// hardiness zones). The USDA bulk CSV is downloaded once and parsed; this job is idempotent
/// and only updates records that still have empty scientific names.
/// USDA bulk download: https://plants.usda.gov/assets/docs/CompletePLANTSList.xlsx
/// </summary>
public class ImportUsdaPlantsJob
{
    private readonly IApplicationDbContext _db;
    private readonly IUsdaPlantsService _usda;
    private readonly ILogger<ImportUsdaPlantsJob> _logger;

    public ImportUsdaPlantsJob(
        IApplicationDbContext db,
        IUsdaPlantsService usda,
        ILogger<ImportUsdaPlantsJob> logger)
    {
        _db = db;
        _usda = usda;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        var plants = await _db.Plants
            .Where(p => p.IsGlobal && (p.ScientificName == string.Empty || p.Family == string.Empty))
            .ToListAsync(ct);

        _logger.LogInformation("Enriching {Count} plants with USDA taxonomy data", plants.Count);
        int enriched = 0;

        foreach (var plant in plants)
        {
            ct.ThrowIfCancellationRequested();
            var data = await _usda.LookupByCommonNameAsync(plant.CommonName, ct);
            if (data == null) continue;

            if (string.IsNullOrEmpty(plant.ScientificName))
                plant.ScientificName = data.ScientificName;
            if (string.IsNullOrEmpty(plant.Family))
                plant.Family = data.Family;
            if (string.IsNullOrEmpty(plant.HardinessZoneMin))
                plant.HardinessZoneMin = data.ZoneMin;
            if (string.IsNullOrEmpty(plant.HardinessZoneMax))
                plant.HardinessZoneMax = data.ZoneMax;

            enriched++;
            await Task.Delay(100, ct);
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("USDA enrichment complete — enriched {Count} plants", enriched);
    }
}
