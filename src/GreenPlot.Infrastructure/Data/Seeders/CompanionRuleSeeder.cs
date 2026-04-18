using GreenPlot.Domain.Entities;
using GreenPlot.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GreenPlot.Infrastructure.Data.Seeders;

/// <summary>
/// Seeds curated companion planting rules from the requirements spec's example dataset.
/// Expand this list with the full Farmer's Almanac/Extension Service dataset before Phase 4 launch.
/// </summary>
public class CompanionRuleSeeder
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<CompanionRuleSeeder> _logger;

    public CompanionRuleSeeder(ApplicationDbContext db, ILogger<CompanionRuleSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await _db.CompanionRules.AnyAsync(ct)) return;

        var tomato = await _db.Plants.FirstOrDefaultAsync(p => p.CommonName == "Tomato", ct);
        var basil = await _db.Plants.FirstOrDefaultAsync(p => p.CommonName == "Basil", ct);
        var corn = await _db.Plants.FirstOrDefaultAsync(p => p.CommonName == "Corn", ct);
        var beans = await _db.Plants.FirstOrDefaultAsync(p => p.CommonName == "Bean", ct);
        var carrot = await _db.Plants.FirstOrDefaultAsync(p => p.CommonName == "Carrot", ct);
        var onion = await _db.Plants.FirstOrDefaultAsync(p => p.CommonName == "Onion", ct);
        var cucumber = await _db.Plants.FirstOrDefaultAsync(p => p.CommonName == "Cucumber", ct);
        var nasturtium = await _db.Plants.FirstOrDefaultAsync(p => p.CommonName == "Nasturtium", ct);

        var rules = new List<(Plant? a, Plant? b, CompanionEffect effect, string reason, CompanionSourceType src)>
        {
            (tomato, basil, CompanionEffect.Good,
                "Basil reported to improve tomato flavor and repel hornworms", CompanionSourceType.Traditional),
            (corn, beans, CompanionEffect.Good,
                "Three-sisters: beans fix nitrogen, corn provides pole support", CompanionSourceType.Traditional),
            (carrot, onion, CompanionEffect.Good,
                "Onion scent disrupts carrot fly; carrots disrupt onion fly", CompanionSourceType.Traditional),
            (cucumber, nasturtium, CompanionEffect.Good,
                "Nasturtium trap crop for aphids and cucumber beetles", CompanionSourceType.Traditional),
            (beans, onion, CompanionEffect.Bad,
                "Alliums inhibit nitrogen-fixing bacteria on bean roots", CompanionSourceType.Traditional),
        };

        foreach (var (a, b, effect, reason, src) in rules)
        {
            if (a == null || b == null) continue;

            _db.CompanionRules.Add(new CompanionRule
            {
                PlantAId = a.Id,
                PlantBId = b.Id,
                Effect = effect,
                Strength = 2,
                Reasoning = reason,
                SourceType = src
            });
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Companion rules seeded");
    }
}
