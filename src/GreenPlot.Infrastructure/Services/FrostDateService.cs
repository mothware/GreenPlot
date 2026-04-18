using GreenPlot.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GreenPlot.Infrastructure.Services;

public class FrostDateService : IFrostDateService
{
    private readonly HttpClient _http;
    private readonly ILogger<FrostDateService> _logger;

    public FrostDateService(HttpClient http, ILogger<FrostDateService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<FrostDateResult?> GetFrostDatesAsync(string zipCode, CancellationToken ct = default)
    {
        try
        {
            // USDA Plant Hardiness Zone API
            var zoneUrl = $"https://phzmapi.org/{zipCode}.json";
            var response = await _http.GetAsync(zoneUrl, ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json);
            var zone = doc.RootElement.GetProperty("zone").GetString() ?? "7a";

            var (lastFrost, firstFrost) = EstimateFrostDatesFromZone(zone);

            return new FrostDateResult(lastFrost, firstFrost, zone);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch frost dates for zip {Zip}", zipCode);
            return null;
        }
    }

    public async Task<string?> GetHardinessZoneAsync(string zipCode, CancellationToken ct = default)
    {
        var result = await GetFrostDatesAsync(zipCode, ct);
        return result?.HardinessZone;
    }

    // Approximate frost dates by hardiness zone — supplemented by NOAA data in Phase 2
    private static (DateOnly lastSpring, DateOnly firstFall) EstimateFrostDatesFromZone(string zone)
    {
        return zone switch
        {
            "3a" or "3b" => (new DateOnly(2026, 5, 25), new DateOnly(2026, 9, 5)),
            "4a" or "4b" => (new DateOnly(2026, 5, 10), new DateOnly(2026, 9, 20)),
            "5a" or "5b" => (new DateOnly(2026, 4, 25), new DateOnly(2026, 10, 5)),
            "6a" or "6b" => (new DateOnly(2026, 4, 10), new DateOnly(2026, 10, 20)),
            "7a" or "7b" => (new DateOnly(2026, 4, 1), new DateOnly(2026, 11, 5)),
            "8a" or "8b" => (new DateOnly(2026, 3, 10), new DateOnly(2026, 11, 20)),
            "9a" or "9b" => (new DateOnly(2026, 2, 15), new DateOnly(2026, 12, 10)),
            "10a" or "10b" or "11a" or "11b" => (new DateOnly(2026, 1, 15), new DateOnly(2026, 12, 25)),
            _ => (new DateOnly(2026, 4, 15), new DateOnly(2026, 10, 25))
        };
    }
}
