using ThePatch.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ThePatch.Infrastructure.Services;

/// <summary>
/// Queries the USDA PLANTS database web service for taxonomy and zone data.
/// USDA does not publish a formal REST API; we use the public data endpoint
/// and fall back to web scraping if needed. This implementation uses the
/// unofficial JSON endpoint that backs the PLANTS search UI.
/// </summary>
public class UsdaPlantsService : IUsdaPlantsService
{
    private readonly HttpClient _http;
    private readonly ILogger<UsdaPlantsService> _logger;

    public UsdaPlantsService(HttpClient http, ILogger<UsdaPlantsService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<UsdaPlantData?> LookupByCommonNameAsync(string commonName, CancellationToken ct = default)
    {
        try
        {
            var encoded = Uri.EscapeDataString(commonName);
            var url = $"https://plants.usda.gov/api/plants?filter=%7B%22common_name%22%3A%22{encoded}%22%7D&fields=Symbol,ScientificName,CommonName,Family,ZoneMin,ZoneMax,NativeStatus&limit=1";

            var response = await _http.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("data", out var data)) return null;
            var arr = data.EnumerateArray().ToList();
            if (arr.Count == 0) return null;

            var item = arr[0];
            return new UsdaPlantData(
                GetString(item, "Symbol") ?? string.Empty,
                GetString(item, "ScientificName") ?? string.Empty,
                GetString(item, "CommonName") ?? commonName,
                GetString(item, "Family") ?? string.Empty,
                GetString(item, "ZoneMin"),
                GetString(item, "ZoneMax"),
                GetString(item, "NativeStatus")
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "USDA lookup failed for: {Name}", commonName);
            return null;
        }
    }

    public async Task<UsdaPlantData?> LookupBySymbolAsync(string symbol, CancellationToken ct = default)
    {
        try
        {
            var url = $"https://plants.usda.gov/api/plants/{symbol}";
            var response = await _http.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync(ct);
            var item = JsonDocument.Parse(json).RootElement;

            return new UsdaPlantData(
                symbol,
                GetString(item, "ScientificName") ?? string.Empty,
                GetString(item, "CommonName") ?? string.Empty,
                GetString(item, "Family") ?? string.Empty,
                GetString(item, "ZoneMin"),
                GetString(item, "ZoneMax"),
                GetString(item, "NativeStatus")
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "USDA symbol lookup failed for: {Symbol}", symbol);
            return null;
        }
    }

    private static string? GetString(JsonElement el, string key) =>
        el.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.String
            ? prop.GetString()
            : null;
}
